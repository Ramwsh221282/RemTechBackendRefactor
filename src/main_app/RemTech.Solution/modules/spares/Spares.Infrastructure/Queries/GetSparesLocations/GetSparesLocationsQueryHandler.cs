using System.Data;
using System.Data.Common;
using Dapper;
using Npgsql;
using Pgvector;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;

namespace Spares.Infrastructure.Queries.GetSparesLocations;

/// <summary>
/// Обработчик запроса на получение локаций запчастей.
/// </summary>
/// <param name="session">Сессия базы данных PostgreSQL.</param>
/// <param name="embeddings">Провайдер эмбеддингов для поиска.</param>
public sealed class GetSparesLocationsQueryHandler(NpgSqlSession session, EmbeddingsProvider embeddings)
	: IQueryHandler<GetSparesLocationsQuery, IEnumerable<SpareLocationResponse>>
{
	private NpgSqlSession Session { get; } = session;
	private EmbeddingsProvider Embeddings { get; } = embeddings;

	/// <summary>
	/// Обрабатывает запрос на получение локаций запчастей.
	/// </summary>
	/// <param name="query">Запрос на получение локаций запчастей.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Коллекция ответов с информацией о локациях запчастей.</returns>
	public async Task<IEnumerable<SpareLocationResponse>> Handle(
		GetSparesLocationsQuery query,
		CancellationToken ct = default
	)
	{
		(DynamicParameters parameters, string sql) = CreateSql(query);
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		NpgsqlConnection connection = await Session.GetConnection(ct);
		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
		return await MapFromReader(reader, ct);
	}

	private static async Task<IReadOnlyList<SpareLocationResponse>> MapFromReader(
		DbDataReader reader,
		CancellationToken ct
	)
	{
		List<SpareLocationResponse> spares = [];
		while (await reader.ReadAsync(ct))
		{
			spares.Add(MapSingleFromReader(reader));
		}

		return spares.AsReadOnly();
	}

	private static string ApplyFilters(
		GetSparesLocationsQuery query,
		DynamicParameters parameters,
		Func<GetSparesLocationsQuery, DynamicParameters, string>[] filterAppliers
	)
	{
		IEnumerable<string> appliers = filterAppliers
			.Select(a => a.Invoke(query, parameters))
			.Where(s => !string.IsNullOrWhiteSpace(s));

		return !appliers.Any() ? string.Empty : " WHERE " + string.Join(" AND ", appliers);
	}

	private static string ApplyOrderBy(
		GetSparesLocationsQuery query,
		DynamicParameters parameters,
		Func<GetSparesLocationsQuery, DynamicParameters, string>[] orderByAppliers
	)
	{
		IEnumerable<string> appliers = orderByAppliers
			.Select(a => a.Invoke(query, parameters))
			.Where(s => !string.IsNullOrWhiteSpace(s));

		return !appliers.Any() ? string.Empty : " ORDER BY " + string.Join(" AND ", appliers);
	}

	private static string UseEmbeddingsSearchOrderBy(GetSparesLocationsQuery query, DynamicParameters parameters)
	{
		return string.IsNullOrWhiteSpace(query.TextSearch) ? string.Empty : "r.embedding <=> @embedding_search ASC";
	}

	private static SpareLocationResponse MapSingleFromReader(DbDataReader reader)
	{
		return new()
		{
			Id = reader.GetGuid(reader.GetOrdinal("Id")),
			Name = reader.GetString(reader.GetOrdinal("Name")),
		};
	}

	private (DynamicParameters Parameters, string Sql) CreateSql(GetSparesLocationsQuery query)
	{
		DynamicParameters parameters = new();
		string sql = $"""
			SELECT DISTINCT				
			    r.id as Id,
			    (r.name || ' ' || r.kind) as Name                        
			FROM vehicles_module.regions r
			    INNER JOIN spares_module.spares s ON s.region_id = r.id
			    INNER JOIN contained_items_module.contained_items ci ON s.id = ci.id AND ci.deleted_at IS NULL
			    {ApplyFilters(query, parameters, [UseTextSearchFilter])}
			    {ApplyOrderBy(query, parameters, [UseEmbeddingsSearchOrderBy])}
				{ApplyAmountLimitFilter(query, parameters)}
			""";
		return (parameters, sql);
	}

	private static string ApplyAmountLimitFilter(GetSparesLocationsQuery query, DynamicParameters parameters)
	{
		if (query.Amount.HasValue)
		{
			parameters.Add("@limit", query.Amount.Value, DbType.Int32);
			return "LIMIT @limit";
		}

		return string.Empty;
	}

	private string UseTextSearchFilter(GetSparesLocationsQuery query, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.TextSearch))
		{
			return string.Empty;
		}

		Vector embeddings = new(Embeddings.Generate(query.TextSearch));
		parameters.Add("@embedding_search", embeddings);
		parameters.Add("@text_search_parameter", query.TextSearch, DbType.String);
		return """
			  (
			    (r.embedding <=> @embedding_search <= 0.6) OR 
			    (r.name ILIKE '%' || @text_search_parameter || '%') OR 
			    (r.kind ILIKE '%' || @text_search_parameter || '%') OR
			    ((r.name || ' ' || r.kind) ILIKE '%' || @text_search_parameter || '%') OR
			    (ts_rank_cd(to_tsvector('russian', r.name), plainto_tsquery('russian', @text_search_parameter)) > 0) OR
			    (ts_rank_cd(to_tsvector('russian', (r.name || ' ' || r.kind)), plainto_tsquery('russian', @text_search_parameter)) > 0) OR
			)
			""";
	}
}
