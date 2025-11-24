using Dapper;
using Mailing.Core.Inbox;
using Mailing.Core.Inbox.Protocols;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace Mailing.Infrastructure.NpgSql.Inbox;

// CREATE TABLE mailing_module.inbox_messages
// (
//     id uuid primary key,
//     recipient_email varchar(256) NOT NULL,
//     subject varchar(128) NOT NULL,
//     body varchar(512) NOT NULL    
// );
public sealed class NpgSqlSaveInboxMessageProtocol(NpgSqlSession session) : SaveInboxMessageProtocol
{
    public async Task Save(InboxMessage message, CancellationToken ct = default)
    {
        const string sql =
            """
            INSERT INTO mailing_module.inbox_messages
            (id, recipient_email, subject, body)
            VALUES
            (@id, @recipient_email, @subject, @body)
            """;
        CommandDefinition command = message.ToCommand(sql, session, ct);
        await session.Execute(command);
    }
}