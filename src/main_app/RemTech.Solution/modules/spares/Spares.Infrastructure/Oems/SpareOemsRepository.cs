using System.Data;
using System.Data.Common;
using Dapper;
using Npgsql;
using Pgvector;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using Spares.Domain.Contracts;
using Spares.Domain.Oems;

namespace Spares.Infrastructure.Oems;

public sealed class SparesOemRepository(NpgSqlSession session, EmbeddingsProvider embeddings, Serilog.ILogger logger)
	: ISpareOemsRepository
{
	private Serilog.ILogger Logger { get; } = logger.ForContext<ISpareOemsRepository>();

	public async Task<SpareOem> SaveOrFindSimilar(SpareOem oem, CancellationToken ct = default)
	{
		int count = await GetAppropriateEfSearchValue(ct);
		Logger.Information("Appropriate hnsw.ef_search value for SpareOem embeddings: {Count}", count);

		await SetHnswlSimilarityCountForCurrentSession(count, ct);
		SpareOem? similarOem = await FindSimilar(oem, ct);
		if (similarOem is not null)
		{
			return similarOem;
		}

		await Save(oem, ct);
		return oem;
	}

	
	public async Task<Dictionary<string, SpareOem>> SaveOrFindManySimilar(
		IEnumerable<SpareOem> oems,
		CancellationToken ct = default
	)
	{
        int amountToAdd = oems.ToArray().Length;
        Logger.Information("Adding {Amount} oems to the database.", amountToAdd);
        
        try
        {
            int count = await GetAppropriateEfSearchValue(ct);
            Logger.Information("Appropriate hnsw.ef_search value for SpareOem embeddings: {Count}", count);
            await SetHnswlSimilarityCountForCurrentSession(count, ct);
            Dictionary<string, SpareOem> added = await FindManySimilarOrNewlyAdded(oems, ct);
            Logger.Information("{Amount} types have been added.", added.Count);
            return added;
        }
        catch(Exception ex)
        {
            Logger.Error(ex, "Error at persisting oems.");
            throw;
        }
	}

	private async Task<Dictionary<string, SpareOem>> FindManySimilarOrNewlyAdded(
		IEnumerable<SpareOem> oems,
		CancellationToken ct = default
	)
	{
		// Это запрос для массового поиска схожих артикулов запчастей.
		// Принип: есть схожие - берем их, нет - добавляем и используем новые, вместе с уже существующими.

		// исходный массив артикулов.
		SpareOem[] originArray = [.. oems];

		// этот словарь нужен для поиска артикулов ПО ВХОДНОМУ ЗНАЧЕНИЮ, ТО ЕСТЬ ТУТ ЕЩЕ НЕ ПОНЯТНО ЕСТЬ ЛИ ОНИ В БД ИЛИ НЕТ
		Dictionary<string, SpareOem> source = originArray.ToDictionary(o => o.Value, o => o);

		// тут словарь ЛИБО НАЙДЕННЫХ (по сравнению значения, триграммы эмбеддинга), ЛИБО СОЗДАННЫХ артикулов (которые не были найдены но будут добавлены)
		Dictionary<string, SpareOem?> found = originArray.ToDictionary(o => o.Value, _ => (SpareOem?)null);

		// тут идет логика для запроса в бд. (будет заполнятся found по сходству с текущими данными)
		await FillWithSimilarOems(originArray, found, ct);

		// здесь подготовка к добавлению того, к чему сходства не было найдено.
		List<SpareOem> toAdd = FilterFromNotAdded(found, source);

		// если есть не найденные, вставляем массово.
		if (toAdd.Count != 0)
		{
			await SaveMany(toAdd, ct);
		}

		// теперь вставленные добавляем в found, т.к нам нужны ИЛИ СХОЖИЕ ИЛИ НОВЫЕ
		return FormMatchingAndNewlyAddedOems(found, toAdd);
	}

	private static Dictionary<string, SpareOem> FormMatchingAndNewlyAddedOems(
		Dictionary<string, SpareOem?> destination,
		List<SpareOem> added
	)
	{
		foreach (SpareOem oem in added)
		{
			destination[oem.Value] = oem;
		}

		return destination.ToDictionary(kv => kv.Key, kv => kv.Value!);
	}

	private static List<SpareOem> FilterFromNotAdded(
		Dictionary<string, SpareOem?> destination,
		Dictionary<string, SpareOem> source
	)
	{
		IEnumerable<KeyValuePair<string, SpareOem?>> notAdded = destination.Where(kv => kv.Value is null);
		List<SpareOem> toAdd = [];
		foreach (KeyValuePair<string, SpareOem?> kv in notAdded)
		{
			SpareOem oemToAdd = source[kv.Key];
			toAdd.Add(oemToAdd);
		}

		return toAdd;
	}

	private async Task FillWithSimilarOems(
		SpareOem[] origins,
		Dictionary<string, SpareOem?> destination,
		CancellationToken ct
	)
	{
		Guid[] inputIdentifiers = [.. origins.Select(o => o.Id.Value)];
		string[] inputTexts = [.. origins.Select(o => o.Value)];
		Vector[] vectors =
		[
			.. embeddings.GenerateBatch([.. origins.Select(PrepareOemEmbeddingText)]).Select(e => new Vector(e)),
		];

		const string sql = """			
			WITH input_oems AS (
				SELECT UNNEST(@input_ids) AS input_id, 
					   UNNEST(@input_texts) AS input_texts,
					   UNNEST(@input_embeddings) AS input_embeddings
			)
			SELECT 
				io.input_id, 
				io.input_texts,
				fo.id AS found_id,
				fo.oem AS found_oem
			FROM input_oems io
			INNER JOIN LATERAL (
				SELECT id, oem FROM spares_module.oems o
				WHERE oem = io.input_texts
				OR similarity(o.oem, io.input_texts) > 0.9
				OR (o.embedding IS NOT NULL AND o.embedding <=> io.input_embeddings < 0.18)
				ORDER BY
					CASE
						WHEN o.oem = io.input_texts THEN 0
						WHEN similarity(o.oem, io.input_texts) > 0.9 THEN 1
						WHEN o.embedding IS NOT NULL AND o.embedding <=> io.input_embeddings < 0.18 THEN 2
						ELSE 3
					END
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
			string value = reader.GetString(reader.GetOrdinal("found_oem"));
			string input = reader.GetString(reader.GetOrdinal("input_texts"));
			SpareOem existing = SpareOem.Create(value, id).Value;
			destination[input] = existing;
		}
	}

    private async Task<IEnumerable<SpareOem>> ExcludeExisting(IEnumerable<SpareOem> oems, CancellationToken token)
    {
        string[] oemsValues = oems.Select(o => o.Value).ToArray();
        const string sql = """
                           SELECT o.oem FROM spares_module.oems o
                           WHERE o.oem = ANY(@oems_values)
                           """;
        
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@oem_values", oemsValues);
        CommandDefinition command = session.FormCommand(sql, parameters, token);
        NpgsqlConnection connection = await session.GetConnection(token);
        await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
        List<string> existing = [];
        while (await reader.ReadAsync(token))
        {
            string value = reader.GetString(reader.GetOrdinal("oem"));
            existing.Add(value);
        }

        return oems.ExceptBy(existing, e => e.Value);
    }
    
	private async Task SaveMany(IEnumerable<SpareOem> oems, CancellationToken ct)
	{
        SpareOem[] itemsToAdd = [.. await ExcludeExisting(oems, ct) ];
        string[] texts = [.. itemsToAdd.Select(i => i.Value)];
        IReadOnlyList<ReadOnlyMemory<float>> vectors = embeddings.GenerateBatch(texts);
        var parameters = itemsToAdd.Select((o, i) => new { id = o.Id.Value, oem = o.Value, embedding = new Vector(vectors[i]) });
        
		const string sql = """
                           INSERT INTO spares_module.oems (id, oem, embedding)
                           VALUES (@id, @oem, @embedding)
                           """;
        
		CommandDefinition command = session.FormCommand(sql, parameters, ct);
		await session.Execute(command);
	}

	private async Task Save(SpareOem oem, CancellationToken ct)
	{
		const string sql = """ 
			INSERT INTO spares_module.oems (id, oem, embedding)
			VALUES (@id, @oem, @embedding)			
			""";

		DynamicParameters parameters = new();
		parameters.Add("@id", oem.Id.Value, DbType.Guid);
		parameters.Add("@oem", oem.Value, DbType.String);
		parameters.Add("@embedding", new Vector(embeddings.Generate(PrepareOemEmbeddingText(oem))));

		CommandDefinition command = session.FormCommand(sql, parameters, ct);
		await session.Execute(command);
	}

	private async Task<SpareOem?> FindSimilar(SpareOem oem, CancellationToken ct)
	{
		const string sql = """			
			WITH exact_search (
				SELECT o.id, o.oem FROM spares_module.oems o
				WHERE o.oem = @oem_text
			)
			trigram_search (
				SELECT o.id, o.oem FROM spares_module.oems o
				WHERE pg_trgm.similarity(o.oem, @oem_text) > 0.9
				ORDER BY pg_trgm.similarity(o.oem, @oem_text) DESC
				LIMIT 1
			),
			embedding_search AS (
				SELECT o.id, o.oem
				FROM spares_module.oems o
				WHERE 
					o.embedding IS NOT NULL AND
					o.embedding <=> @oem_embedding < 0.18
				ORDER BY o.embedding <=> @oem_embedding ASC
				LIMIT 1
			)
			SELECT es.id as es_id, es.oem as es_oem FROM exact_search es
			UNION
			SELECT ts.id as ts_id, ts.oem as ts_oem FROM trigram_search ts
			UNION
			SELECT emb.id as emb_id, emb.oem as emb_oem FROM embedding_search emb
			LIMIT 1;
			""";

		string oemText = oem.Value;
		Vector oemEmbedding = new(embeddings.Generate(PrepareOemEmbeddingText(oem)));
		DynamicParameters parameters = new();
		parameters.Add("@oem_text", oemText, DbType.String);
		parameters.Add("@oem_embedding", oemEmbedding);
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
			string value = reader.GetString(reader.GetOrdinal("es_oem"));
			Logger.Information("Found exact match SpareOem: {Oem} ({Id})", value, id);
			return SpareOem.Create(value, id);
		}

		if (!await reader.IsDBNullAsync(reader.GetOrdinal("ts_id"), cancellationToken: ct))
		{
			Guid id = reader.GetGuid(reader.GetOrdinal("ts_id"));
			string value = reader.GetString(reader.GetOrdinal("ts_oem"));
			Logger.Information("Found trigram match SpareOem: {Oem} ({Id})", value, id);
			return SpareOem.Create(value, id);
		}

		if (!await reader.IsDBNullAsync(reader.GetOrdinal("emb_id"), cancellationToken: ct))
		{
			Guid id = reader.GetGuid(reader.GetOrdinal("emb_id"));
			string value = reader.GetString(reader.GetOrdinal("emb_oem"));
			Logger.Information("Found embedding match SpareOem: {Oem} ({Id})", value, id);
			return SpareOem.Create(value, id);
		}

		Logger.Information("No similar SpareOem found for: {Oem}", oem.Value);
		return null;
	}

	private async Task SetHnswlSimilarityCountForCurrentSession(int count, CancellationToken ct)
	{
        
		//const string sql = "SET LOCAL hnsw.ef_search = @count::text";
		const string sql = "SELECT set_config('hnsw.ef_search', @count::text, true);";
        DynamicParameters parameters = new();
        parameters.Add("@count", count, DbType.Int32);
		CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
		await session.Execute(command);
	}

	private async Task<int> GetAppropriateEfSearchValue(CancellationToken ct)
	{
		const string sql = """
			SELECT LEAST(
				GREATEST((0.5 * COUNT(*)::integer), 50), 1000
			) FROM spares_module.oems
			""";
		CommandDefinition command = new(sql, cancellationToken: ct, transaction: session.Transaction);
		return await session.QuerySingleRow<int>(command);
	}

	private static string PrepareOemEmbeddingText(SpareOem oem)
	{
		return $"Артикул запчасти: {oem.Value.Trim()}";
	}
}
