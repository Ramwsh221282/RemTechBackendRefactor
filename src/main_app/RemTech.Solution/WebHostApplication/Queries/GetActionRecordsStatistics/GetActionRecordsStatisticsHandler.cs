using System.Data;
using System.Data.Common;
using System.Text.Json;
using Dapper;
using Npgsql;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using WebHostApplication.CommonSql;
using WebHostApplication.Queries.GetActionRecords;
using WebHostApplication.Queries.Responses;

namespace WebHostApplication.Queries.GetActionRecordsStatistics;

/// <summary>
/// Обработчик запроса на получение статистики записей действий.
/// </summary>
/// <param name="session">Сессия базы данных</param>
/// <param name="embeddings">Провайдер эмбеддингов</param>
public sealed class GetActionRecordsStatisticsHandler(NpgSqlSession session, EmbeddingsProvider embeddings)
	: IQueryHandler<GetActionRecordsQuery, IReadOnlyList<ActionRecordsStatisticsResponse>>
{
	private NpgSqlSession Session { get; } = session;
	private EmbeddingsProvider Embeddings { get; } = embeddings;

	/// <summary>
	/// Обрабатывает запрос на получение статистики записей действий.
	/// </summary>
	/// <param name="query">Запрос на получение статистики записей действий</param>
	/// <param name="ct">Токен отмены</param>
	/// <returns>Ответ с статистикой записей действий</returns>
	public async Task<IReadOnlyList<ActionRecordsStatisticsResponse>> Handle(
		GetActionRecordsQuery query,
		CancellationToken ct = default
	)
	{
		(DynamicParameters parameters, string sql) = CreateSql(query);
		NpgsqlConnection connection = await Session.GetConnection(ct);
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
		return await CreateFromReader(reader, ct);
	}

	private (DynamicParameters parameters, string sql) CreateSql(GetActionRecordsQuery query)
	{
		DynamicParameters parameters = new();
		string mainQueryFilters = BuildMainQuerySqlFilter(query, parameters);
		string sql = $"""
SELECT
    jsonb_agg (
        jsonb_build_object (
            'Date',
            inner_query.Date,
            'Amount',
            inner_query.Amount
        )
    ) as result
FROM
    (
        SELECT
            Date (ar.created_at) as Date,
            COUNT(*) as Amount
        FROM
            telemetry_module.action_records ar
        LEFT JOIN 
            identity_module.accounts a ON ar.invoker_id = a.id
        {mainQueryFilters}
        GROUP BY
            Date (ar.created_at)
        ORDER BY
            Date
    ) as inner_query
""";
		return (parameters, sql);
	}

	private string BuildMainQuerySqlFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		return SqlBuilderDelegate.CombineWhereClauses(query, parameters, []);
	}

	private static async Task<IReadOnlyList<ActionRecordsStatisticsResponse>> CreateFromReader(
		DbDataReader reader,
		CancellationToken ct
	)
	{
		List<ActionRecordsStatisticsResponse> results = [];
		while (await reader.ReadAsync(ct))
		{
			IReadOnlyList<ActionRecordsStatisticsResponse> records = JsonSerializer.Deserialize<
				IReadOnlyList<ActionRecordsStatisticsResponse>
			>(reader.GetString(reader.GetOrdinal("result")))!;
			results.AddRange(records);
		}

		return results;
	}
}
