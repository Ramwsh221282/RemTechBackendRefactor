using System.Data;
using Dapper;
using Mailing.Core.Inbox;
using RemTech.NpgSql.Abstractions;

namespace Mailing.Infrastructure.NpgSql.Inbox;

public static class NpgSqlInboxMessageManagement
{
    extension(InboxMessage message)
    {
        public CommandDefinition ToCommand(string sql, NpgSqlSession session, CancellationToken ct)
        {
            DynamicParameters parameters = message.FillParameters();
            CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
            return command;
        }
        
        private DynamicParameters FillParameters()
        {
            DynamicParameters parameters = new();
            parameters.Add("@id", Guid.NewGuid(), DbType.Guid);
            parameters.Add("@recipient_email", message.TargetEmail.Value, DbType.String);
            parameters.Add("@subject", message.Subject.Value, DbType.String);
            parameters.Add("@body", message.Body.Value, DbType.String);
            return parameters;
        }
    }
}