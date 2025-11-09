using System.Data;
using Dapper;
using Mailers.Core.MailersContext;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Persistence.NpgSql;

public static class NpgSqlMailerSendings
{
    extension(MailerSending sending)
    {
        public async Task Insert(NpgSqlSession session, CancellationToken ct = default)
        {
            const string sql
                = """
                  INSERT INTO mailers_module.mailer_sendings
                  (id, mailer_id, subject, body, sent_to, sent_on)
                  VALUES    
                  (@id, @mailer_id, @subject, @body, @sent_to, @sent_on)
                  """;
            await session.Execute(sending.CreateCommand(sql, session, ct));
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