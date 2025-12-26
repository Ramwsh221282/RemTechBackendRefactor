using System.Data;
using System.Text.Json;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Telemetry.Domain.Models;
using Telemetry.Domain.Models.ValueObjects;
using Telemetry.Domain.Ports.Storage;

namespace Telemetry.Adapter.Storage.Storage;

public sealed partial class ActionRecordsStorage
{
    public async Task<ActionRecordsCollection> GetCollection(
        ReadActions parameters,
        CancellationToken ct = default
    )
    {
        DynamicParameters sqlParams = new DynamicParameters();
        List<string> whereClauses = [];
        List<string> orderByClauses = [];
        List<string> paginationClauses = [];

        if (parameters.InvokerId != null)
        {
            whereClauses.Add("invoker_id = @invoker_id");
            sqlParams.Add("@invoker_id", parameters.InvokerId.Value.Value, DbType.Guid);
        }

        if (parameters.OccuredAtMax != null)
        {
            whereClauses.Add("occured_at <= @maxDate");
            sqlParams.Add("@maxDate", parameters.OccuredAtMax.Value.OccuredAt, DbType.DateTime);
        }

        if (parameters.OccuredAtMin != null)
        {
            whereClauses.Add("occured_at >= @minDate");
            sqlParams.Add("@minDate", parameters.OccuredAtMin.Value.OccuredAt, DbType.DateTime);
        }

        if (parameters.Name != null)
        {
            whereClauses.Add("name = @name");
            sqlParams.Add("@name", parameters.Name.Value, DbType.String);
        }

        if (!string.IsNullOrWhiteSpace(parameters.TextSearch))
        {
            Vector vector = new Vector(generator.Generate(parameters.TextSearch));
            whereClauses.Add("embedding <=> @embedding <= @threshold");
            orderByClauses.Add("embedding <=> @embedding");
            sqlParams.Add("@embedding", vector);
            sqlParams.Add("@threshold", 0.7);
        }

        if (parameters.Status != null)
        {
            whereClauses.Add("status = @status");
            sqlParams.Add("@status", parameters.Status.Value, DbType.String);
        }

        if (parameters.OrderNames != null)
        {
            foreach (string orderName in parameters.OrderNames)
            {
                string mode = parameters.OrderMode;
                switch (orderName)
                {
                    case "occured_at":
                        orderByClauses.Add($"occured_at {mode}");
                        break;
                    case "status":
                        orderByClauses.Add($"status {mode}");
                        break;
                    case "name":
                        orderByClauses.Add($"name {mode}");
                        break;
                }
            }
        }

        int limit = parameters.PageSize;
        int offset = (parameters.Page - 1) * limit;
        paginationClauses.Add("LIMIT @limit");
        paginationClauses.Add("OFFSET @offset");
        sqlParams.Add("@limit", limit, DbType.Int32);
        sqlParams.Add("@offset", offset, DbType.Int32);

        string whereClause =
            whereClauses.Count == 0 ? string.Empty : " WHERE " + string.Join(" AND ", whereClauses);
        string orderClause =
            orderByClauses.Count == 0
                ? string.Empty
                : " ORDER BY " + string.Join(", ", orderByClauses);
        string paginationClause =
            paginationClauses.Count == 0 ? string.Empty : string.Join("  ", paginationClauses);

        string sql = $"""
            SELECT
            id,
            invoker_id,
            status as status,
            name,
            comments -> 'Name' ->> 'Value' as name,
            occured_at as occured_at,
            COUNT(*) OVER() as total_count
            FROM telemetry_module.records
            {whereClause}
            {orderClause}
            {paginationClause}
            """;

        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        using IDbConnection connection = context.Database.GetDbConnection();
        long totalCount = -1;

        IEnumerable<dynamic> results = await connection.QueryAsync(command);
        IEnumerable<ActionRecord> records = results.Select(r =>
        {
            if (totalCount == -1)
                totalCount = r["total_count"];
            return new ActionRecord
            {
                Id = ActionId.Create(r["id"]),
                InvokerId = ActionInvokerId.Create(r["invoker_id"]),
                Status = ActionStatus.Create(r["status"]),
                Name = ActionName.Create(r["name"]),
                OccuredAt = ActionDate.Create(r["occured_at"]),
                Comments = ActionComment.Create(
                    JsonSerializer.Deserialize<IEnumerable<string>>(r["comments"])
                ),
            };
        });

        return new ActionRecordsCollection(totalCount, records);
    }
}
