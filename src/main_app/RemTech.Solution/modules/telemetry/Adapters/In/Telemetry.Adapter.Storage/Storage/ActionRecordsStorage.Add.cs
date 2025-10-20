using System.Data;
using System.Text.Json;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Telemetry.Adapter.Storage.DataModels;
using Telemetry.Adapter.Storage.Extensions;
using Telemetry.Domain.Models;

namespace Telemetry.Adapter.Storage.Storage;

public sealed partial class ActionRecordsStorage
{
    public async Task Add(ActionRecord record, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO telemetry_module.records
                (id, invoker_id, name, status, occured_at, comments, embedding)
            VALUES
                (@id, @invoker_id, @name, @status, @occured_at, @comments, @embedding)
            """;

        ActionRecordDataModel data = record.ConvertToDataModel();
        Vector vector = data.GenerateEmbedding(generator);
        DynamicParameters parameters = new();
        parameters.Add("@id", data.Id, DbType.Guid);
        parameters.Add("@invoker_id", data.InvokerId, DbType.Guid);
        parameters.Add("@name", data.Name, DbType.String);
        parameters.Add("@status", data.Status, DbType.String);
        parameters.Add("@occured_at", data.OccuredAt, DbType.DateTime);
        parameters.Add(
            "@comments",
            JsonSerializer.Serialize(data.Comments, JsonSerializerOptions.Default)
        );
        parameters.Add("@embedding", vector);

        IDbConnection connection = context.Database.GetDbConnection();
        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        await connection.ExecuteAsync(command);
    }
}
