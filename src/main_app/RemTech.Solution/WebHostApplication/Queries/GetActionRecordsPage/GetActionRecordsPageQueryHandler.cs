using System.Data.Common;
using System.Text.Json;
using Dapper;
using Identity.Infrastructure.Permissions.GetPermissions;
using Npgsql;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using WebHostApplication.Queries.GetActionRecords;
using WebHostApplication.Queries.Responses;

namespace WebHostApplication.Queries.GetActionRecordsPage;

public sealed class GetActionRecordsPageQueryHandler(NpgSqlSession session, EmbeddingsProvider embeddings)
	: IQueryHandler<GetActionRecordsQuery, ActionRecordsPageResponse>
{
	public async Task<ActionRecordsPageResponse> Handle(GetActionRecordsQuery query, CancellationToken ct = default)
	{
		(DynamicParameters parameters, string sql) = CreateSql(query);
		NpgsqlConnection connection = await session.GetConnection(ct);
		CommandDefinition command = session.FormCommand(sql, parameters, ct);
		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
		return await CreateFromReader(reader, ct);
	}

	private static async Task<ActionRecordsPageResponse> CreateFromReader(DbDataReader reader, CancellationToken ct)
	{
		List<ActionRecordsPageResponse> result = [];
		GetActionRecordQueryResponse records = new();
		while (await reader.ReadAsync(ct))
		{
			IReadOnlyList<ActionRecordsStatisticsResponse> statistics = JsonSerializer.Deserialize<
				IReadOnlyList<ActionRecordsStatisticsResponse>
			>(reader.GetString(reader.GetOrdinal("statistics")))!;

			IReadOnlyList<PermissionResponse> permissions = JsonSerializer.Deserialize<
				IReadOnlyList<PermissionResponse>
			>(reader.GetString(reader.GetOrdinal("permissions")))!;

			IReadOnlyList<ActionRecordStatusResponse> statuses = JsonSerializer.Deserialize<
				IReadOnlyList<ActionRecordStatusResponse>
			>(reader.GetString(reader.GetOrdinal("statuses")))!;

			int totalCount = reader.GetInt32(reader.GetOrdinal("total_count"));

			IReadOnlyList<ActionRecordResponse> items = JsonSerializer.Deserialize<IReadOnlyList<ActionRecordResponse>>(
				reader.GetString(reader.GetOrdinal("items"))
			)!;

			records.TotalCount = totalCount;
			records.Items = items;

			ActionRecordsPageResponse response = new()
			{
				Records = records,
				Statistics = statistics,
				Permissions = permissions,
				Statuses = statuses,
			};

			result.Add(response);
		}

		return result.Count == 0
			? new ActionRecordsPageResponse()
			{
				Permissions = [],
				Records = new GetActionRecordQueryResponse(),
				Statistics = [],
				Statuses = [],
			}
			: result[0];
	}

	private (DynamicParameters parameters, string sql) CreateSql(GetActionRecordsQuery query)
	{
		DynamicParameters parameters = new();
		string sql = $"""
        WITH statistics_query AS (
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
        {GetActionRecordsStatisticsQueryBuilder.CreateFilterSql(query, parameters)}      
        GROUP BY
            Date (ar.created_at)
        ORDER BY
            Date
    ) inner_query
),
statuses_query AS (
    SELECT
        jsonb_agg (
            jsonb_build_object (
                'Status', iq.Status
            )
        ) as result
    FROM
        (
            SELECT
                DISTINCT(ar.severity) as Status
            FROM
                telemetry_module.action_records ar
        ) iq
),
user_permissions AS (
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
    {GetActionRecordsQueryBuilder.CreateFilterSql(parameters, query, embeddings)}
    {GetActionRecordsQueryBuilder.CreatePaginationSql(parameters, query)}
),
results AS (
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
FROM items i
)
SELECT sq.result as statistics,
       up.permissions as permissions,
       stq.result as statuses,
       items.Items as items,
       items.TotalCount as total_count
FROM statistics_query sq
CROSS JOIN user_permissions up
CROSS JOIN statuses_query stq
CROSS JOIN results items;
""";
		return (parameters, sql);
	}
}
