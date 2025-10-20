// using System.Data;
// using Dapper;
// using Microsoft.EntityFrameworkCore;
// using Pgvector;
// using Shared.Infrastructure.Module.Postgres.Embeddings;
// using Telemetry.Domain.Models;
// using Telemetry.Domain.Models.ValueObjects;
// using Telemetry.Domain.Ports.Storage;
// using Telemetry.Domain.TelemetryContext;
//
// namespace Telemetry.Infrastructure.PostgreSQL.Repositories.Read;
//
// public sealed class ActionRecordsStorage(
//     TelemetryServiceDbContext context,
//     IEmbeddingGenerator generator
// ) : IActionRecordsStorage
// {
//     public async Task<(IEnumerable<ActionRecord> Records, long Count)> Read(
//         int page = 1,
//         int pageSize = 20,
//         ActionInvokerId? invokerId = null,
//         ActionStatus? status = null,
//         ActionName? name = null,
//         ActionDate? occuredAtMin = null,
//         ActionDate? occuredAtMax = null,
//         string? textSearch = null,
//         IEnumerable<string>? orderNames = null,
//         string orderMode = "ASC",
//         CancellationToken ct = default
//     )
//     {
//         DynamicParameters parameters = new DynamicParameters();
//         List<string> whereClauses = [];
//         List<string> orderByClauses = [];
//         List<string> paginationClauses = [];
//
//         if (invokerId != null)
//         {
//             whereClauses.Add("invoker_id = @invoker_id");
//             parameters.Add("@invoker_id", invokerId.Value.Value, DbType.Guid);
//         }
//
//         if (occuredAtMax != null)
//         {
//             whereClauses.Add("occured_at <= @maxDate");
//             parameters.Add("@maxDate", occuredAtMax.Value.OccuredAt, DbType.DateTime);
//         }
//
//         if (occuredAtMin != null)
//         {
//             whereClauses.Add("occured_at >= @minDate");
//             parameters.Add("@minDate", occuredAtMin.Value.OccuredAt, DbType.DateTime);
//         }
//
//         if (name != null)
//         {
//             whereClauses.Add("details -> 'Name' ->> 'Value' = @name");
//             parameters.Add("@name", name.Value, DbType.String);
//         }
//
//         if (!string.IsNullOrWhiteSpace(textSearch))
//         {
//             Vector vector = new Vector(generator.Generate(textSearch));
//             whereClauses.Add("embedding <=> @embedding <= @threshold");
//             orderByClauses.Add("embedding <=> @embedding");
//             parameters.Add("@embedding", vector);
//             parameters.Add("@threshold", 0.7);
//         }
//
//         if (status != null)
//         {
//             whereClauses.Add("status = @status");
//             parameters.Add("@status", status.Value, DbType.String);
//         }
//
//         if (orderNames != null)
//         {
//             foreach (string orderName in orderNames)
//             {
//                 switch (orderName)
//                 {
//                     case "occured_at":
//                         orderByClauses.Add($"occured_at {orderMode}");
//                         break;
//                     case "status":
//                         orderByClauses.Add($"status {orderMode}");
//                         break;
//                     case "name":
//                         orderByClauses.Add($"name {orderMode}");
//                         break;
//                 }
//             }
//         }
//
//         int limit = pageSize;
//         int offset = (page - 1) * limit;
//         paginationClauses.Add("LIMIT @limit");
//         paginationClauses.Add("OFFSET @offset");
//         parameters.Add("@limit", limit, DbType.Int32);
//         parameters.Add("@offset", offset, DbType.Int32);
//
//         string whereClause =
//             whereClauses.Count == 0 ? string.Empty : " WHERE " + string.Join(" AND ", whereClauses);
//         string orderClause =
//             orderByClauses.Count == 0
//                 ? string.Empty
//                 : " ORDER BY " + string.Join(", ", orderByClauses);
//         string paginationClause =
//             paginationClauses.Count == 0 ? string.Empty : string.Join("  ", paginationClauses);
//
//         string sql = $"""
//             SELECT
//             id as id,
//             invoker_id as invoker_id,
//             status as status,
//             details -> 'Name' ->> 'Value' as name,
//             details as comments,
//             occured_at as occured_at,
//             COUNT(*) OVER() as total_count
//             FROM telemetry_module.records
//             {whereClause}
//             {orderClause}
//             {paginationClause}
//             """;
//
//         CommandDefinition command = new(sql, parameters, cancellationToken: ct);
//         using IDbConnection connection = context.Database.GetDbConnection();
//         long totalCount = -1;
//
//         IEnumerable<dynamic> results = await connection.QueryAsync(command);
//         List<ActionRecord> records = results
//             .Select(r => new ActionRecord(
//                 ActionId.Create(r.id),
//                 ActionInvokerId.Create(r.invoker_id),
//                 new TelemetryActionDetails(
//                     ActionName.Create(r.name),
//                     Enumerable.Empty<ActionComment>()
//                 ),
//                 ActionStatus.Create(r.status),
//                 ActionDate.Create(r.occured_at)
//             ))
//             .ToList();
//
//         return (records, totalCount);
//     }
// }
