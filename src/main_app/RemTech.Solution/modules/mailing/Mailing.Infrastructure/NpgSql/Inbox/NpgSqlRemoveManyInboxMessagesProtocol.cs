using Dapper;
using Mailing.Core.Inbox;
using Mailing.Infrastructure.InboxMessageProcessing.Protocols;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace Mailing.Infrastructure.NpgSql.Inbox;

public sealed class NpgSqlRemoveManyInboxMessagesProtocol(NpgSqlSession session) : RemoveManyInboxMessagesProtocol
{
    public async Task Remove(IEnumerable<InboxMessage> messages, CancellationToken ct)
    {
        const string sql = "DELETE FROM mailing_module.inbox_messages WHERE id = ANY(@ids)";
        Guid[] ids = messages.Select(x => x.Id).ToArray();
        DynamicParameters parameters = new();
        parameters.Add("@ids", ids);
        CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
        await session.Execute(command);
    }
}