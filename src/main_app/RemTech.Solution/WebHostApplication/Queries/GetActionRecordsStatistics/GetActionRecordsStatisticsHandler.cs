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

	// TODO: permissions fitlter should be added in main query using EXISTS.
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

	private static string UseLoginSearch(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.LoginSearch))
		{
			return string.Empty;
		}

		parameters.Add("@ST_LoginSearch", query.LoginSearch, DbType.String);
		return "a.login ILIKE '%' || @ST_LoginSearch || '%'";
	}

	private static string UseEmailSearch(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.EmailSearch))
		{
			return string.Empty;
		}

		parameters.Add("@ST_EmailSearch", query.EmailSearch, DbType.String);
		return "a.email ILIKE '%' || @ST_EmailSearch || '%'";
	}

	private string UseActionNameSearch(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.ActionNameSearch))
		{
			return string.Empty;
		}

		parameters.Add("@ST_ActionNameSearch", query.ActionNameSearch, DbType.String);
		return """
			(
				ar.name ILIKE '%' || @ST_ActionNameSearch || '%' OR
				ts_rank_cd(ts_vector_field, to_tsquery('russian', @ST_ActionNameSearch))) > 0 OR
				(embedding_vector <=> @ST_ActionNameSearchEmbedding) < 0.5
			)
			""";
	}

	private string UseDateRangeFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (query.StartDate == null || query.EndDate == null)
		{
			return string.Empty;
		}

		parameters.Add("@ST_StartDate", query.StartDate.Value, DbType.DateTime);
		parameters.Add("@ST_EndDate", query.EndDate.Value, DbType.DateTime);
		return "ar.created_at BETWEEN @ST_StartDate AND @ST_EndDate";
	}

	private static string UseSubQueryPermissionsFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		List<string> filters = ["ap.account_id = a.id"];
		if (query.PermissionIdentifiers?.Any() != true)
		{
			return string.Empty;
		}

		Guid[] ids = [.. query.PermissionIdentifiers];
		filters.Add("ap.permission_id = ANY(@ST_PermissionIds)");
		parameters.Add("@ST_PermissionIds", ids);
		return string.Join(" AND ", filters);
	}

	private static string UseOperationStatusesFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (query.StatusNames?.Any() != true)
		{
			return string.Empty;
		}

		string[] statusNames = [.. query.StatusNames];
		parameters.Add("@ST_StatusNames", statusNames);
		return "ar.severity = ANY(@ST_StatusNames)";
	}

	private static string UseIgnoreSelfFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (query.IdOfRequestInvoker == null)
		{
			return string.Empty;
		}

		parameters.Add("@ST_InvokerId", query.IdOfRequestInvoker.Value, DbType.Guid);
		return "ar.invoker_id <> @ST_InvokerId";
	}

	private static string UseIgnoreErrorsFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (!query.IgnoreErrors)
		{
			return string.Empty;
		}

		return "ar.error IS NULL";
	}

	private string BuildMainQuerySqlFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		return SqlBuilderDelegate.CombineWhereClauses(
			query,
			parameters,
			[
				UseLoginSearch,
				UseEmailSearch,
				UseActionNameSearch,
				UseDateRangeFilter,
				UseOperationStatusesFilter,
				UseIgnoreErrorsFilter,
				UseIgnoreSelfFilter,
			]
		);
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
