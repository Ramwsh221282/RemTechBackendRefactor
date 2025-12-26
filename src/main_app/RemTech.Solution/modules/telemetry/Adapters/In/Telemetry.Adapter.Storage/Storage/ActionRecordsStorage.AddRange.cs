using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Telemetry.Adapter.Storage.DataModels;
using Telemetry.Adapter.Storage.Extensions;
using Telemetry.Domain.Models;

namespace Telemetry.Adapter.Storage.Storage;

public sealed partial class ActionRecordsStorage
{
    public async Task<IEnumerable<ActionRecord>> AddRange(
        IEnumerable<ActionRecord> records,
        CancellationToken ct = default
    )
    {
        const string sql = """
            INSERT INTO telemetry_module.records
                (id, invoker_id, name, status, occured_at, comments, embedding)
            VALUES
                (@id, @invoker_id, @name, @status, @occured_at, @comments, @embedding)
            """;

        IEnumerable<ActionRecordDataModel> dataModels = records.Select(r => r.ConvertToDataModel());
        var parameters = dataModels.Select(m => new
        {
            id = m.Id,
            invoker_id = m.InvokerId,
            name = m.Name,
            status = m.Status,
            occured_at = m.OccuredAt,
            comments = m.Comments,
            embedding = m.GenerateEmbedding(generator),
        });

        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        IDbConnection connection = context.Database.GetDbConnection();
        await connection.ExecuteAsync(command);
        return records;
    }
}
