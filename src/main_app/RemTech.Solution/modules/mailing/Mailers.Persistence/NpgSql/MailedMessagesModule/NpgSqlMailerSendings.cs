using System.Data;
using Dapper;
using Mailers.Core.MailersModule;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Persistence.NpgSql.MailedMessagesModule;

public static class NpgSqlMailerSendings
{
    public static InsertMailerSending InsertMailerSending(NpgSqlSession session) => (sending, ct) => sending.Insert(session, ct);
    
    extension(MailerSending sending)
    {
        private async Task<Result<Unit>> Insert(NpgSqlSession session, CancellationToken ct = default)
        {
            const string sql
                = """
                  INSERT INTO mailers_module.mailer_sendings
                  (id, mailer_id, subject, body, sent_to, sent_on)
                  VALUES    
                  (@id, @mailer_id, @subject, @body, @sent_to, @sent_on)
                  """;
            await session.Execute(sending.CreateCommand(sql, session, ct));
            return Unit.Value;
        }

        private CommandDefinition CreateCommand(string sql, NpgSqlSession session, CancellationToken ct = default)
        {
            return new CommandDefinition(
                sql, 
                sending.FillParameters(), 
                cancellationToken: ct, 
                transaction: session.Transaction);
        }
        
        private DynamicParameters FillParameters()
        {
            var messageInfo = sending.Message;
            DynamicParameters parameters = new();
            parameters.Add("@id", messageInfo.Metadata.MessageId, DbType.Guid);
            parameters.Add("@mailer_id", messageInfo.Metadata.MailerId, DbType.Guid);
            parameters.Add("@subject", messageInfo.Content.Subject, DbType.String);
            parameters.Add("@body", messageInfo.Content.Body, DbType.String);
            parameters.Add("@sent_to", messageInfo.DeliveryInfo.To.Value, DbType.String);
            parameters.Add("@sent_on", messageInfo.DeliveryInfo.SentOn, DbType.DateTime);
            return parameters;
        }
    }
}