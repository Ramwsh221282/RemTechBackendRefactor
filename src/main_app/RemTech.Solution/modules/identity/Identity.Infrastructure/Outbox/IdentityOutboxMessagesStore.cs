using System.Data;

namespace Identity.Infrastructure.Outbox;

public sealed class IdentityOutboxMessagesStore(NpgSqlSession session)
{
    public async Task Add(IdentityOutboxMessage message, CancellationToken ct = default)
    {
        const string sql =
            """
            INSERT INTO identity_module.outbox
            (id, type, payload, created, was_sent)
            VALUES
            (@id, @type, @payload::jsonb, @created, @was_sent)
            """;
        DynamicParameters parameters = FillParameters(message);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        await session.Execute(command);
    }

    public async Task<IdentityOutboxMessage[]> GetUnsentMessages(CancellationToken ct = default, int amount = 50)
    {
        const string sql = "SELECT * FROM identity_module.outbox WHERE was_sent = FALSE LIMIT @limit FOR UPDATE";
        CommandDefinition command = new(sql, new { limit = amount }, cancellationToken: ct, transaction: session.Transaction);
        IEnumerable<IdentityOutboxMessage> messages = await session.QueryMultipleRows<IdentityOutboxMessage>(command);
        return messages.ToArray();
    }

    public async Task UpdateMessages(IdentityOutboxMessage[] messages, CancellationToken ct = default)
    {
        const string sql = "UPDATE identity_module.outbox  SET was_sent = TRUE where id = ANY(@ids)";
        DynamicParameters parameters = CreateParametersWithOnlyIdsArray(messages);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        await session.Execute(command);
    }

    public async Task RemoveMessages(IdentityOutboxMessage[] messages, CancellationToken ct = default)
    {
        const string sql = "DELETE FROM identity_module.outbox WHERE id = ANY(@ids)";
        DynamicParameters parameters = CreateParametersWithOnlyIdsArray(messages);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        await session.Execute(command);
    }

    public async Task<IdentityOutboxMessage?> GetByType(string type, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM identity_module.outbox WHERE type = @type LIMIT 1";
        DynamicParameters parameters = new();
        parameters.Add("@type", type, DbType.String);
        CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
        IdentityOutboxMessage? message = await session.QueryMaybeRow<IdentityOutboxMessage?>(command);
        return message;
    }
    
    private static DynamicParameters CreateParametersWithOnlyIdsArray(IdentityOutboxMessage[] messages)
    {
        Guid[] ids = messages.Select(x => x.Id).ToArray();
        DynamicParameters parameters = new();
        parameters.Add("@ids", ids);
        return parameters;
    }
    
    private static DynamicParameters FillParameters(IdentityOutboxMessage message)
    {
        DynamicParameters parameters = new();
        parameters.Add("@id", message.Id, DbType.Guid);
        parameters.Add("@type", message.Type, DbType.String);
        parameters.Add("@payload", message.Payload);
        parameters.Add("@created", message.Created);
        parameters.Add("@was_sent", message.WasSent);
        return parameters;
    }
}