using System.Data;
using System.Data.Common;
using Dapper;
using Npgsql;
using Pgvector;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;

namespace Spares.Infrastructure.Queries.GetSpareTypes;

/// <summary>
/// Обработчик запроса на получение типов запчастей.
/// </summary>
/// <param name="session">Сессия базы данных PostgreSQL.</param>
/// <param name="embeddings">Провайдер эмбеддингов для обработки текстовых запросов.</param>
public sealed class GetSpareTypesQueryHandler(NpgSqlSession session, EmbeddingsProvider embeddings)
	: IQueryHandler<GetSpareTypesQuery, IEnumerable<SpareTypeResponse>>
{
	private NpgSqlSession Session { get; } = session;
	private EmbeddingsProvider Embeddings { get; } = embeddings;

	/// <summary>
	/// Обрабатывает запрос на получение типов запчастей.
	/// </summary>
	/// <param name="query">Запрос на получение типов запчастей.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Коллекция ответов с типами запчастей.</returns>
	public async Task<IEnumerable<SpareTypeResponse>> Handle(GetSpareTypesQuery query, CancellationToken ct = default)
	{
		(DynamicParameters parameters, string sql) = CreateSql(query);
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		NpgsqlConnection connection = await Session.GetConnection(ct);
		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
		return await MapFromReader(reader, ct);
	}

	private static async Task<IReadOnlyList<SpareTypeResponse>> MapFromReader(DbDataReader reader, CancellationToken ct)
	{
		List<SpareTypeResponse> responses = [];
		while (await reader.ReadAsync(ct))
		{
			responses.Add(new SpareTypeResponse { Value = reader.GetString("Type") });
		}

		return responses;
	}

	private static string UseLimit(GetSpareTypesQuery query, DynamicParameters parameters)
	{
		if (query.Amount is null)
		{
			return string.Empty;
		}

		parameters.Add("@limit", query.Amount, DbType.Int32);
		return "LIMIT @limit";
	}

	private static string UseFilters(
		GetSpareTypesQuery query,
		DynamicParameters parameters,
		Func<GetSpareTypesQuery, DynamicParameters, string>[] filters
	)
	{
		IEnumerable<string> clauses = filters
			.Select(f => f.Invoke(query, parameters))
			.Where(s => !string.IsNullOrWhiteSpace(s));

		if (!clauses.Any())
		{
			return string.Empty;
		}

		return "WHERE " + string.Join(" AND ", clauses);
	}

	private (DynamicParameters Parameters, string Sql) CreateSql(GetSpareTypesQuery query)
	{
		DynamicParameters parameters = new();
		string sql = $"""
			SELECT
				DISTINCT (s.type) AS Type
			FROM
				spares_module.spares s
			{UseFilters(query, parameters, [UseTextSearchFilter])}			
			{UseLimit(query, parameters)}
			""";

		return (parameters, sql);
	}

	private string UseTextSearchFilter(GetSpareTypesQuery query, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.TextSearch))
		{
			return string.Empty;
		}

		Vector vector = new(Embeddings.Generate(query.TextSearch));
		parameters.Add("@embedding", vector);
		parameters.Add("@text_search_parameter", query.TextSearch, DbType.String);
		return """ 
			(
			s.embedding <=> @embedding OR
			s.type ILIKE '%' || @text_search_parameter || '%' OR
			ts_rank_cd(to_tsvector('russian', s.type), to_tsquery('russian', @text_search_parameter)) > 0
			)
			""";
	}
}
