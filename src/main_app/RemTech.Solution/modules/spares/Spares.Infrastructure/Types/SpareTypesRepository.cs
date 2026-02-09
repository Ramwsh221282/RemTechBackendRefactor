using System.Data;
using System.Data.Common;
using Dapper;
using Npgsql;
using Pgvector;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using Spares.Domain.Contracts;
using Spares.Domain.Types;

namespace Spares.Infrastructure.Types;

public sealed class SpareTypesRepository(NpgSqlSession session, EmbeddingsProvider embeddings, Serilog.ILogger logger)
	: ISpareTypesRepository
{
	private Serilog.ILogger Logger { get; } = logger.ForContext<ISpareTypesRepository>();

	public async Task<SpareType> SaveOrFindSimilar(SpareType type, CancellationToken ct = default)
	{
		int count = await GetAppropriateEfSearchValue(ct);
		Logger.Information("Appropriate hnsw.ef_search value for SpareType embeddings: {Count}", count);

		await SetHnswlSimilarityCountForCurrentSession(count, ct);
		SpareType? similarType = await FindSimilar(type, ct);
		if (similarType is not null)
		{
			return similarType;
		}

		await Save(type, ct);
		return type;
	}

	public async Task<Dictionary<string, SpareType>> SaveOrFindManySimilar(
		IEnumerable<SpareType> types,
		CancellationToken ct = default
	)
    {
        int amountToAdd = types.ToArray().Length;
        Logger.Information("Adding {Amount} types to the database.", amountToAdd);
        try
        {
            int count = await GetAppropriateEfSearchValue(ct);
            Logger.Information("Appropriate hnsw.ef_search value for SpareType embeddings: {Count}", count);
            await SetHnswlSimilarityCountForCurrentSession(count, ct);
            Dictionary<string, SpareType> added = await FindManySimilarOrNewlyAdded(types, ct);
            Logger.Information("{Amount} types have been added.", added.Count);
            return added;
        }
        catch(Exception ex)
        {
            Logger.Error(ex, "Error at perstisting types");
            throw;
        }
	}

	private async Task<Dictionary<string, SpareType>> FindManySimilarOrNewlyAdded(
		IEnumerable<SpareType> types,
		CancellationToken ct = default
	)
	{
		SpareType[] origins = [.. types];
		Dictionary<string, SpareType> source = origins.ToDictionary(t => t.Value, t => t);
		Dictionary<string, SpareType?> found = origins.ToDictionary(t => t.Value, _ => (SpareType?)null);
		await FillWithSimilarTypes(origins, found, ct);
		List<SpareType> toAdd = CreateListOfNotExistingTypes(found, source);
		if (toAdd.Count != 0)
		{
			await SaveMany(toAdd, ct);
		}

		return FormMatchingAndNewlyAddedTypes(found, toAdd);
	}

	private static Dictionary<string, SpareType> FormMatchingAndNewlyAddedTypes(
		Dictionary<string, SpareType?> found,
		List<SpareType> added
	)
	{
		foreach (SpareType type in added)
		{
			found[type.Value] = type;
		}

		return found.ToDictionary(kv => kv.Key, kv => kv.Value!);
	}
    
	private async Task SaveMany(IEnumerable<SpareType> types, CancellationToken ct = default)
    {
        SpareType[] onlyNew = [..await FilterTypesFromExisting(types, ct)];
        string[] texts = [..types.Select(t => t.Value)];
        IReadOnlyList<ReadOnlyMemory<float>> vectors = embeddings.GenerateBatch(texts);
        var parameters = onlyNew.Select((t, i) => new { id = t.Id.Value, type = t.Value, embedding = new Vector(vectors[i]) });
        
		const string sql = """ 
			INSERT INTO spares_module.types (id, type, embedding)
			VALUES (@id, @type, @embedding)			
			""";
        
		CommandDefinition command = session.FormCommand(sql, parameters, ct);
		await session.Execute(command);
	}

	private static List<SpareType> CreateListOfNotExistingTypes(
		Dictionary<string, SpareType?> destination,
		Dictionary<string, SpareType> source
	)
	{
		IEnumerable<KeyValuePair<string, SpareType?>> notAdded = destination.Where(kv => kv.Value is null);
		List<SpareType> toAdd = [];
		foreach (KeyValuePair<string, SpareType?> kv in notAdded)
		{
			SpareType typeToAdd = source[kv.Key];
			toAdd.Add(typeToAdd);
		}

		return toAdd;
	}

	private async Task FillWithSimilarTypes(
		SpareType[] origins,
		Dictionary<string, SpareType?> destination,
		CancellationToken ct = default
	)
	{
		Guid[] inputIdentifiers = [.. origins.Select(o => o.Id.Value)];
		string[] inputTexts = [.. origins.Select(o => o.Value)];
		Vector[] vectors =
		[
			.. embeddings.GenerateBatch([.. origins.Select(type => type.Value.Trim())]).Select(e => new Vector(e)),
		];

		const string sql = """			
			WITH input_types AS (
			    SELECT UNNEST(@input_ids) AS input_id,
			           UNNEST(@input_texts) AS input_texts,
			           UNNEST(@input_embeddings) AS input_embeddings
			)
			SELECT
			    io.input_id,
			    io.input_texts,
			    fo.id AS found_id,
			    fo.type AS found_type
			FROM input_types io
			         INNER JOIN LATERAL (
			    WITH filtered_by_vector AS (
			        SELECT 
			            id, 
			            type,
			            1 - (t.embedding <=> io.input_embeddings) as score
			            FROM spares_module.types t
			        WHERE 
			            t.embedding IS NOT NULL
			            AND 1 - (t.embedding <=> io.input_embeddings) >= 0.4
			        ORDER BY score DESC
			        LIMIT 20
			    )
			    SELECT 
			        filtered_by_vector.id, 
			        filtered_by_vector.type,
			        similarity(filtered_by_vector.type, io.input_texts) as sml
			    FROM 
			        filtered_by_vector
			    WHERE
			        filtered_by_vector.type = io.input_texts
			        OR similarity(filtered_by_vector.type, io.input_texts) >= 0.8       
			    ORDER BY
			        CASE
			            WHEN filtered_by_vector.type = io.input_texts THEN 0
			            WHEN similarity(filtered_by_vector.type, io.input_texts) >= 0.8 THEN 1            
			            ELSE 2
			            END,
			        sml DESC
			    LIMIT 1
			    ) fo ON TRUE;
			""";

		DynamicParameters parameters = new();
		parameters.Add("@input_ids", inputIdentifiers);
		parameters.Add("@input_texts", inputTexts);
		parameters.Add("@input_embeddings", vectors);
		CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
		NpgsqlConnection connection = await session.GetConnection(ct);

		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
		while (await reader.ReadAsync(ct))
		{
			Guid id = reader.GetGuid(reader.GetOrdinal("found_id"));
			string value = reader.GetString(reader.GetOrdinal("found_type"));
			string input = reader.GetString(reader.GetOrdinal("input_texts"));
			SpareType existing = SpareType.Create(value, id).Value;
			destination[input] = existing;
		}
	}

    private async Task<IEnumerable<SpareType>> FilterTypesFromExisting(
        IEnumerable<SpareType> types,
        CancellationToken ct)
    {
        SpareType[] originArray = [..types];
        string[] typeTexts = [.. originArray.Select(t => t.Value)];
        const string sql = """
                           SELECT t.type FROM spares_module.types t 
                           WHERE t.type = ANY(@type_texts)
                           """;

        DynamicParameters parameters = new();
        parameters.Add("@type_texts", typeTexts);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        NpgsqlConnection connection = await session.GetConnection(ct);
        await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
        List<string> existingTypes = [];
        while (await reader.ReadAsync(ct))
        {
            string type = reader.GetString(reader.GetOrdinal("type"));
            existingTypes.Add(type);
        }

        return originArray.ExceptBy(existingTypes, t => t.Value);
    }

	private async Task Save(SpareType type, CancellationToken ct)
	{
		const string sql = """ 
			INSERT INTO spares_module.types (id, type, embedding)
			VALUES (@id, @type, @embedding)			
			""";

		DynamicParameters parameters = new();
		parameters.Add("@id", type.Id.Value, DbType.Guid);
		parameters.Add("@type", type.Value, DbType.String);
		parameters.Add("@embedding", new Vector(embeddings.Generate(type.Value.Trim())));

		CommandDefinition command = session.FormCommand(sql, parameters, ct);
		await session.Execute(command);
	}

	private async Task<SpareType?> FindSimilar(SpareType type, CancellationToken ct)
	{
		const string sql = """			
			WITH exact_search (
				SELECT t.id, t.type FROM spares_module.types t
				WHERE t.type = @type_text
			)
			trigram_search (
				SELECT t.id, t.type FROM spares_module.types t
				WHERE pg_trgm.similarity(t.type, @type_text) > 0.9
				ORDER BY pg_trgm.similarity(t.type, @type_text) DESC
				LIMIT 1
			),
			embedding_search AS (
				SELECT t.id, t.type
				FROM spares_module.types t
				WHERE 
					t.embedding IS NOT NULL AND
					t.embedding <=> @type_embedding < 0.18
				ORDER BY t.embedding <=> @type_embedding ASC
				LIMIT 1
			)
			SELECT es.id as es_id, es.type as es_type FROM exact_search es
			UNION
			SELECT ts.id as ts_id, ts.type as ts_type FROM trigram_search ts
			UNION
			SELECT emb.id as emb_id, emb.type as emb_type FROM embedding_search emb
			LIMIT 1;
			""";

		string typeText = type.Value;
		Vector typeEmbedding = new(embeddings.Generate(type.Value.Trim()));
		DynamicParameters parameters = new();
		parameters.Add("@type_text", typeText, DbType.String);
		parameters.Add("@type_embedding", typeEmbedding);
		CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
		NpgsqlConnection connection = await session.GetConnection(ct);
		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
		if (!await reader.ReadAsync(ct))
		{
			return null;
		}

		if (!await reader.IsDBNullAsync(reader.GetOrdinal("es_id"), cancellationToken: ct))
		{
			Guid id = reader.GetGuid(reader.GetOrdinal("es_id"));
			string value = reader.GetString(reader.GetOrdinal("es_type"));
			Logger.Information("Found exact match SpareType: {Type} ({Id})", value, id);
			return SpareType.Create(value, id);
		}

		if (!await reader.IsDBNullAsync(reader.GetOrdinal("ts_id"), cancellationToken: ct))
		{
			Guid id = reader.GetGuid(reader.GetOrdinal("ts_id"));
			string value = reader.GetString(reader.GetOrdinal("ts_type"));
			Logger.Information("Found trigram match SpareType: {Type} ({Id})", value, id);
			return SpareType.Create(value, id);
		}

		if (!await reader.IsDBNullAsync(reader.GetOrdinal("emb_id"), cancellationToken: ct))
		{
			Guid id = reader.GetGuid(reader.GetOrdinal("emb_id"));
			string value = reader.GetString(reader.GetOrdinal("emb_type"));
			Logger.Information("Found embedding match SpareType: {Type} ({Id})", value, id);
			return SpareType.Create(value, id);
		}

		Logger.Information("No similar SpareType found for: {Type}", type.Value);
		return null;
	}

	private async Task<int> GetAppropriateEfSearchValue(CancellationToken ct)
	{
		const string sql = """
			SELECT LEAST(
				GREATEST((0.5 * COUNT(*)::integer), 50), 1000
			) FROM spares_module.types
			""";
		CommandDefinition command = new(sql, cancellationToken: ct, transaction: session.Transaction);
		return await session.QuerySingleRow<int>(command);
	}

	private async Task SetHnswlSimilarityCountForCurrentSession(int count, CancellationToken ct)
	{
		//const string sql = "SET LOCAL hnsw.ef_search = @count;";
        const string sql = "SELECT set_config('hnsw.ef_search', @count::text, true);";
		CommandDefinition command = new(sql, new { count }, cancellationToken: ct, transaction: session.Transaction);
		await session.Execute(command);
	}
}
