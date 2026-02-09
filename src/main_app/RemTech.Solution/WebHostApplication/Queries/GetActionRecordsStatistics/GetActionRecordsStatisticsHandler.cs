using System.Data;
using System.Data.Common;
using System.Text.Json;
using Dapper;
using Npgsql;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;
using WebHostApplication.CommonSql;
using WebHostApplication.Queries.GetActionRecords;
using WebHostApplication.Queries.Responses;

namespace WebHostApplication.Queries.GetActionRecordsStatistics;

/// <summary>
/// Обработчик запроса на получение статистики записей действий.
/// </summary>
/// <param name="session">Сессия базы данных</param>
/// <param name="embeddings">Провайдер эмбеддингов</param>
public sealed class GetActionRecordsStatisticsHandler(NpgSqlSession session)
	: IQueryHandler<GetActionRecordsQuery, IReadOnlyList<ActionRecordsStatisticsResponse>>
{
	private NpgSqlSession Session { get; } = session;

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

	private static (DynamicParameters parameters, string sql) CreateSql(GetActionRecordsQuery query)
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

	/// <summary>
	/// Строит SQL фильтр для основного запроса.
	/// </summary>
	private static string BuildMainQuerySqlFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		return SqlBuilderDelegate.CombineWhereClauses(
			query,
			parameters,
			[DateRangeFilter(), WeekDateRangeFilter()]
		);
	}

    private static WhereClauseBuilderDelegate<GetActionRecordsQuery> WeekDateRangeFilter()
    {
        return (query, _) => query.UsingWeek.HasValue && query.UsingWeek.Value
            ? "Date (ar.created_at) >= Date (CURRENT_DATE - INTERVAL '7 days')"
            : string.Empty;
    }

    private static WhereClauseBuilderDelegate<GetActionRecordsQuery> DateRangeFilter()
    {
        return (query, parameters) =>
        {
            List<string> clauses = [];

            if (query.UsingWeek.HasValue && query.UsingWeek.Value)
            {
                return string.Empty;
            }

            if (query.ChartStartDate.HasValue)
            {
                clauses.Add("ar.created_at >= @StartDate");
                parameters.Add("StartDate", query.ChartStartDate.Value, DbType.DateTime);
            }

            if (query.ChartEndDate.HasValue)
            {
                clauses.Add("ar.created_at <= @EndDate");
                parameters.Add("EndDate", query.ChartEndDate.Value, DbType.DateTime);
            }

            return clauses.Count == 0 ? string.Empty : string.Join(" AND ", clauses);
        };
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
