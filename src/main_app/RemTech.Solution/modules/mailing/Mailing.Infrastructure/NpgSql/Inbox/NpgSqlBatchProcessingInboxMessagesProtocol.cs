using System.Data;
using Mailing.Core.Common;
using Mailing.Core.Inbox;
using Mailing.Infrastructure.InboxMessageProcessing.Protocols;

namespace Mailing.Infrastructure.NpgSql.Inbox;

public sealed class NpgSqlBatchProcessingInboxMessagesProtocol(NpgSqlSession session) : GetPendingInboxMessagesProtocol
{
    private const int messagesLimit = 50;
    
    public async IAsyncEnumerable<InboxMessage> GetPendingMessages(CancellationToken ct)
    {
        const string sql = """
                           SELECT
                           id,
                           recipient_email,
                           subject,
                           body
                           FROM mailing_module.inbox_messages 
                           WHERE id > @lastId 
                           ORDER BY id 
                           LIMIT @limit
                           """;
        Guid lastId = Guid.Empty;
        while (true)
        {
            DynamicParameters parameters = new();
            parameters.Add("@limit", messagesLimit, DbType.Int32);
            parameters.Add("@lastId", lastId, DbType.Guid);
            CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
            IEnumerable<TableInboxMessage> messages = await session.QueryMultipleRows<TableInboxMessage>(command);
            TableInboxMessage[] array = messages.ToArray();
            if (array.Length == 0) yield break;
            lastId = array[^1].Id;
            foreach (TableInboxMessage message in array)
            {
                yield return message.ToInboxMessage();
            }
        }
    }
    
    // CREATE TABLE mailing_module.inbox_messages
    // (
    // id uuid primary key,
    //     recipient_email varchar(256) NOT NULL,
    //     subject varchar(128) NOT NULL,
    //     body varchar(512) NOT NULL    
    // );
    private sealed record TableInboxMessage(Guid Id, string RecipientEmail, string Subject, string Body)
    {
        public InboxMessage ToInboxMessage()
        {
            Email email = new(RecipientEmail);
            MessageSubject subject = new(Subject);
            MessageBody body = new(Body);
            return new InboxMessage(Id, email, subject, body);
        }
    }
}