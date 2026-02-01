using System.Data;
using System.Data.Common;
using System.Text.Json;
using Dapper;
using Npgsql;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using WebHostApplication.CommonSql;
using WebHostApplication.Injection;
using WebHostApplication.Queries.Responses;

namespace WebHostApplication.Queries.GetActionRecords;

/// <summary>
/// Обработчик запроса <see cref="GetActionRecordsQuery"/> .
/// </summary>
/// <param name="session">Сессия NpgSql (PostgreSQL connection).</param>
/// <param name="embeddings">Провайдер эмбеддингов.</param>
[UseCache]
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
		return await CreateFromReader(reader, ct);
	}

	private (DynamicParameters parameters, string sql) CreateSql(GetActionRecordsQuery query)
	{
		DynamicParameters parameters = new();
		string mainQueryFilters = BuildMainQuerySqlFilter(query, parameters);
		string paginationQueryPart = BuildPaginationQueryPart(query, parameters);

		string sql = $"""
WITH user_permissions AS (
    SELECT
        ap.account_id,
        jsonb_agg(
            jsonb_build_object('Name', p.name, 'Description', p.description, 'Id', p.id)
        ) AS permissions
    FROM identity_module.account_permissions ap
    LEFT JOIN identity_module.permissions p ON p.id = ap.permission_id
    GROUP BY ap.account_id
),
items AS (
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
        COALESCE(up.permissions, '[]') as UserPermissions
    FROM telemetry_module.action_records ar
    LEFT JOIN identity_module.accounts a ON ar.invoker_id = a.id
    LEFT JOIN user_permissions up ON up.account_id = a.id
    {mainQueryFilters}
	{paginationQueryPart}
)
SELECT 
    MAX(i.TotalCount) as TotalCount, 
    jsonb_agg(
        jsonb_build_object(
            'Id', i.Id,
            'UserId', i.UserId,
            'UserLogin', i.UserLogin,
            'UserEmail', i.UserEmail,
            'UserPermissions', i.UserPermissions,
            'ActionName', i.ActionName,
            'ActionSeverity', i.ActionSeverity,
            'ErrorMessage', i.ErrorMessage,
            'ActionTimestamp', i.ActionTimestamp
        )
    ) as Items
FROM items i;
""";
		return (parameters, sql);
	}

	private static string UseLoginSearch(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.LoginSearch))
		{
			return string.Empty;
		}

		parameters.Add("@LoginSearch", query.LoginSearch, DbType.String);
		return "a.login ILIKE '%' || @LoginSearch || '%'";
	}

	private static string UseEmailSearch(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.EmailSearch))
		{
			return string.Empty;
		}

		parameters.Add("@EmailSearch", query.EmailSearch, DbType.String);
		return "a.email ILIKE '%' || @EmailSearch || '%'";
	}

	private string UseActionNameSearch(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.ActionNameSearch))
		{
			return string.Empty;
		}

		parameters.Add("@ActionNameSearch", query.ActionNameSearch, DbType.String);
		return """
			(
				ar.name ILIKE '%' || @ActionNameSearch || '%' OR
				ts_rank_cd(ts_vector_field, plainto_tsquery('russian', @ActionNameSearch))) > 0 OR
				(embedding_vector <=> @ActionNameSearchEmbedding) < 0.5
			)
			""";
	}

	private string UseDateRangeFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (query.StartDate == null || query.EndDate == null)
		{
			return string.Empty;
		}

		parameters.Add("@StartDate", query.StartDate.Value, DbType.DateTime);
		parameters.Add("@EndDate", query.EndDate.Value, DbType.DateTime);
		return "ar.created_at BETWEEN @StartDate AND @EndDate";
	}

	private static string UseSubQueryPermissionsFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		List<string> filters = ["ap.account_id = a.id"];
		if (query.PermissionIdentifiers?.Any() != true)
		{
			return string.Empty;
		}

		Guid[] ids = [.. query.PermissionIdentifiers];
		filters.Add("ap.permission_id = ANY(@PermissionIds)");
		parameters.Add("@PermissionIds", ids);
		return string.Join(" AND ", filters);
	}

	private static string UserOperationStatusFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.Status))
		{
			return string.Empty;
		}

		parameters.Add("@statusName", query.Status, DbType.String);
		return "ar.severity = @statusName";
	}

	private static string UseIgnoreSelfFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (query.IdOfRequestInvoker == null)
		{
			return string.Empty;
		}

		parameters.Add("@InvokerId", query.IdOfRequestInvoker.Value, DbType.Guid);
		return "ar.invoker_id <> @InvokerId";
	}

	private static string UseIgnoreErrorsFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		return !query.IgnoreErrors ? string.Empty : "ar.error IS NULL";
	}

	//TODO: implement permissions filter
	private static string UsePermissionsFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		return string.Empty;
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
				UserOperationStatusFilter,
				UseIgnoreErrorsFilter,
				UseIgnoreSelfFilter,
				UsePermissionsFilter,
			]
		);
	}

	private static async Task<GetActionRecordQueryResponse> CreateFromReader(DbDataReader reader, CancellationToken ct)
	{
		GetActionRecordQueryResponse response = new();
		bool isTotalCountSet = false;
		while (await reader.ReadAsync(ct))
		{
			IReadOnlyList<ActionRecordResponse> records = JsonSerializer.Deserialize<
				IReadOnlyList<ActionRecordResponse>
			>(reader.GetString(reader.GetOrdinal("Items")))!;

			if (!isTotalCountSet && records.Count > 0)
			{
				response.TotalCount = reader.GetInt32(reader.GetOrdinal("TotalCount"));
				isTotalCountSet = true;
			}

			response.Items = records;
		}

		return response;
	}

	private static string BuildPaginationQueryPart(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		return SqlBuilderDelegate.BuildPaginationClause(query, parameters, q => q.Page, q => q.PageSize);
	}
}
