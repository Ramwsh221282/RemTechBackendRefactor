using Dapper;
using Mailing.Core.Inbox;
using Mailing.Infrastructure.InboxMessageProcessing.Protocols;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace Mailing.Infrastructure.NpgSql.Inbox;

public sealed class NpgSqlMarkProcessedMessagesProtocol(NpgSqlSession session)
: MarkProcessedMessagesProtocol
{
    public async Task Mark(IEnumerable<InboxMessage> messages, CancellationToken ct)
    {
        const string sql = "UPDATE mailing_module.inbox_messages SET has_processed = TRUE WHERE id = ANY(@ids)";
        Guid[] ids = messages.Select(x => x.Id).ToArray();
        DynamicParameters parameters = new();
        parameters.Add("@ids", ids);
        CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
        await session.Execute(command);
    }
}