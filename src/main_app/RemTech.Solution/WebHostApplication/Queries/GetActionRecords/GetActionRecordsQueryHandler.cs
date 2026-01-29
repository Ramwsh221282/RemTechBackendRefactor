using System.Data;
using System.Data.Common;
using System.Text.Json;
using Dapper;
using Npgsql;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using WebHostApplication.CommonSql;

namespace WebHostApplication.Queries.GetActionRecords;

/// <summary>
/// Обработчик запроса <see cref="GetActionRecordsQuery"/> .
/// </summary>
/// <param name="session">Сессия NpgSql (PostgreSQL connection).</param>
/// <param name="embeddings">Провайдер эмбеддингов.</param>
public sealed class GetActionRecordsQueryHandler(NpgSqlSession session, EmbeddingsProvider embeddings)
	: IQueryHandler<GetActionRecordsQuery, GetActionRecordQueryResponse>
{
	private NpgSqlSession Session { get; } = session;
	private EmbeddingsProvider Embeddings { get; } = embeddings;

	/// <summary>
	/// Обрабатывает запрос на получение записей действий.
	/// </summary>
	/// <param name="query">Запрос на получение записей действий.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Список записей действий.</returns>
	public async Task<GetActionRecordQueryResponse> Handle(GetActionRecordsQuery query, CancellationToken ct = default)
	{
		(DynamicParameters parameters, string sql) = CreateSql(query);
		NpgsqlConnection session = await Session.GetConnection(ct);
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		await using DbDataReader reader = await session.ExecuteReaderAsync(command);
		return await CreateFromReader(query, reader, ct);
	}

	private (DynamicParameters parameters, string sql) CreateSql(GetActionRecordsQuery query)
	{
		DynamicParameters parameters = new();
		string mainQueryFilters = BuildMainQuerySqlFilter(query, parameters);
		string permissionsSubQueryFilter = BuildPermissionsSubQueryFilter(query, parameters);

		string sql = $"""
WITH items as (
	SELECT
	ar.id as Id,
	a.login as UserLogin,
	a.email as UserEmail,
	a.id as UserId,
	ar.name as ActionName,
	ar.severity as ActionSeverity,
	ar.error as ErrorMessage,
	ar.created_at as ActionTimestamp,
	COUNT(*) OVER() as TotalCount,
	(
		SELECT
			COALESCE(
				jsonb_agg (
					jsonb_build_object ('Name', p.name, 'Description', p.description, 'Id', p.id)
				),
				'[]'
			)
		FROM
			identity_module.account_permissions ap
			LEFT JOIN identity_module.permissions p ON p.id = ap.permission_id
		{permissionsSubQueryFilter}
	) as UserPermissions
	FROM
telemetry_module.action_records ar
	LEFT JOIN identity_module.accounts a ON ar.invoker_id = a.id
{mainQueryFilters}
)
SELECT
Date(i.ActionTimestamp) as DateByDay,
jsonb_agg(jsonb_build_object('Id', i.Id, 
'UserId', i.UserId, 
'UserLogin', i.UserLogin, 
'UserEmail', i.UserEmail, 
'UserPermissions', i.UserPermissions, 
'ActionName', i.ActionName, 
'ActionSeverity', i.ActionSeverity, 
'ErrorMessage', i.ErrorMessage,
'ActionTimestamp', i.ActionTimestamp,
'TotalCount', i.TotalCount)) as results
FROM items i
GROUP BY DateByDay
ORDER BY DateByDay
""";
		return (parameters, sql);
	}

	private static string UseLoginSearch(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.LoginSearch))
			return string.Empty;

		parameters.Add("@LoginSearch", query.LoginSearch, DbType.String);
		return "a.login ILIKE '%' || @LoginSearch || '%'";
	}

	private static string UseEmailSearch(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.EmailSearch))
			return string.Empty;

		parameters.Add("@EmailSearch", query.EmailSearch, DbType.String);
		return "a.email ILIKE '%' || @EmailSearch || '%'";
	}

	private string UseActionNameSearch(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.ActionNameSearch))
			return string.Empty;

		parameters.Add("@ActionNameSearch", query.ActionNameSearch, DbType.String);
		return """
			(
				ar.name ILIKE '%' || @ActionNameSearch || '%' OR
				ts_rank_cd(ts_vector_field, to_tsquery('russian', @ActionNameSearch))) > 0 OR
				(embedding_vector <=> @ActionNameSearchEmbedding) < 0.5
			)
			""";
	}

	private string UseDateRangeFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (query.StartDate == null || query.EndDate == null)
			return string.Empty;

		parameters.Add("@StartDate", query.StartDate.Value, DbType.DateTime);
		parameters.Add("@EndDate", query.EndDate.Value, DbType.DateTime);
		return "ar.created_at BETWEEN @StartDate AND @EndDate";
	}

	private static string UseSubQueryPermissionsFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		List<string> filters = ["ap.account_id = a.id"];
		if (query.PermissionIdentifiers?.Any() != true)
			return string.Empty;

		Guid[] ids = [.. query.PermissionIdentifiers];
		filters.Add("ap.permission_id = ANY(@PermissionIds)");
		parameters.Add("@PermissionIds", ids);
		return string.Join(" AND ", filters);
	}

	private static string UseOperationStatusesFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (query.StatusNames?.Any() != true)
			return string.Empty;

		string[] statusNames = [.. query.StatusNames];
		parameters.Add("@StatusNames", statusNames);
		return "ar.severity = ANY(@StatusNames)";
	}

	private static string UseIgnoreSelfFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (query.IdOfRequestInvoker == null)
			return string.Empty;

		parameters.Add("@InvokerId", query.IdOfRequestInvoker.Value, DbType.Guid);
		return "ar.invoker_id <> @InvokerId";
	}

	private static string UseIgnoreErrorsFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (!query.IgnoreErrors)
			return string.Empty;

		return "ar.error IS NULL";
	}

	private static string BuildPermissionsSubQueryFilter(GetActionRecordsQuery query, DynamicParameters parameters) =>
		SqlBuilderDelegate.CombineWhereClauses(query, parameters, UseSubQueryPermissionsFilter);

	private string BuildMainQuerySqlFilter(GetActionRecordsQuery query, DynamicParameters parameters) =>
		SqlBuilderDelegate.CombineWhereClauses(
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

	private static async Task<GetActionRecordQueryResponse> CreateFromReader(
		GetActionRecordsQuery query,
		DbDataReader reader,
		CancellationToken ct
	)
	{
		List<ActionRecordAnalyticsResponse> results = [];
		GetActionRecordQueryResponse response = new();
		bool isTotalCountSet = false;
		while (await reader.ReadAsync(ct))
		{
			IReadOnlyList<ActionRecordResponse> records = JsonSerializer.Deserialize<
				IReadOnlyList<ActionRecordResponse>
			>(reader.GetString(reader.GetOrdinal("results")))!;

			if (!isTotalCountSet && records.Count > 0)
			{
				response.TotalCount = records[0].TotalCount;
				isTotalCountSet = true;
			}

			DateTime dateByDay = reader.GetDateTime(reader.GetOrdinal("DateByDay"));
			results.Add(new ActionRecordAnalyticsResponse { DateByDay = dateByDay, Results = records });
		}

		response.Items = results;
		return response;
	}

	// private static async Task<ActionRecordResponse> CreateSingleFromReader(DbDataReader reader, CancellationToken ct)
	// {
	// 	return new ActionRecordResponse()
	// 	{
	// 		UserLogin = await reader.IsDBNullAsync(reader.GetOrdinal("user_login"), ct)
	// 			? null
	// 			: reader.GetString(reader.GetOrdinal("user_login")),
	// 		UserEmail = await reader.IsDBNullAsync(reader.GetOrdinal("user_email"), ct)
	// 			? null
	// 			: reader.GetString(reader.GetOrdinal("user_email")),
	// 		ErrorMessage = await reader.IsDBNullAsync(reader.GetOrdinal("error_message"), ct)
	// 			? null
	// 			: reader.GetString(reader.GetOrdinal("error_message")),
	// 		UserId = await reader.IsDBNullAsync(reader.GetOrdinal("user_id"), ct)
	// 			? null
	// 			: reader.GetGuid(reader.GetOrdinal("user_id")),
	// 		UserPermissions = await ActionRecordUserPermissionResponse.ArrayFromDbReader(reader, ct),
	// 		Id = reader.GetGuid(reader.GetOrdinal("record_id")),
	// 		ActionTimestamp = reader.GetDateTime(reader.GetOrdinal("action_timestamp")),
	// 		ActionName = reader.GetString(reader.GetOrdinal("action_name")),
	// 		ActionSeverity = reader.GetString(reader.GetOrdinal("action_severity")),
	// 	};
	// }
}
