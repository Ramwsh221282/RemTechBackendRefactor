using System.Text.Json;
using Dapper;
using Identity.Domain.Contracts;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Identity.Infrastructure.Common;

public sealed class AccountsModuleOutbox(NpgSqlSession session) : IAccountModuleOutbox
{
    private NpgSqlSession Session { get; } = session;
    
    public async Task Add<TMessage>(
        OutboxMessage<TMessage> message, 
        CancellationToken ct = default)
            where TMessage : IOutboxMessagePayload
    {
        const string sql = """
                           INSERT INTO
                           identity_module.outbox
                           (id, type, retry_count, created, sent, payload)
                           VALUES
                           (@id, @type, @retry_count, @created, @sent, @payload::jsonb)
                           """;

        var parameters = new
        {
            id = message.Id,
            type = message.Type,
            retry_count = message.RetryCount,
            created = message.Created,
            sent = message.Sent.HasValue ? message.Sent.Value : (object?)null,
            payload = JsonSerializer.Serialize(message.Payload)
        };

        CommandDefinition command = Session.FormCommand(sql, parameters, ct);
        await Session.Execute(command);
    }
}