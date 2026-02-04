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

	private async Task Save(SpareType type, CancellationToken ct)
	{
		const string sql = """ 
			INSERT INTO spares_module.types (id, type, embedding)
			VALUES (@id, @type, @embedding)			
			""";

		DynamicParameters parameters = new();
		parameters.Add("@id", type.Id.Value, DbType.Guid);
		parameters.Add("@type", type.Value, DbType.String);
		parameters.Add("@embedding", new Vector(embeddings.Generate(PrepareTypeEmbeddingText(type))));

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
		Vector typeEmbedding = new(embeddings.Generate(PrepareTypeEmbeddingText(type)));
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
		const string sql = "SET LOCAL hnsw.ef_search = @count;";
		CommandDefinition command = new(sql, new { count }, cancellationToken: ct, transaction: session.Transaction);
		await session.Execute(command);
	}

	private static string PrepareTypeEmbeddingText(SpareType type)
	{
		return $"Тип запчасти: {type.Value.Trim()}";
	}
}
