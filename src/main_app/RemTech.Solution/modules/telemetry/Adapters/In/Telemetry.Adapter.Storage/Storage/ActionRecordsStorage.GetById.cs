using System.Data;
using System.Text.Json;
using Dapper;
using Microsoft.EntityFrameworkCore;
using RemTech.Core.Shared.Result;
using Telemetry.Domain.Models;
using Telemetry.Domain.Models.ValueObjects;

namespace Telemetry.Adapter.Storage.Storage;

public sealed partial class ActionRecordsStorage
{
    public async Task<Status<ActionRecord>> GetById(
        ActionId recordId,
        CancellationToken ct = default
    )
    {
        const string sql = """
            SELECT
                id,
                invoker_id,
                status,
                name,
                occured_at,
                comments
            FROM telemetry_module.records
            WHERE id = @id
            """;

        CommandDefinition command = new(sql, new { id = recordId.Value }, cancellationToken: ct);
        IDbConnection connection = context.Database.GetDbConnection();

        var data = await connection.QueryFirstOrDefaultAsync(command);
        if (data == null)
            return Error.NotFound($"Запись с ID: {recordId} не найдена.");

        return new ActionRecord
        {
            Id = ActionId.Create(data["id"]),
            InvokerId = ActionInvokerId.Create(data["invoker_id"]),
            Status = ActionStatus.Create(data["status"]),
            Name = ActionName.Create(data["name"]),
            OccuredAt = ActionDate.Create(data["occured_at"]),
            Comments = ActionComment.Create(
                JsonSerializer.Deserialize<IEnumerable<string>>(data["comments"])
            ),
        };
    }
}
