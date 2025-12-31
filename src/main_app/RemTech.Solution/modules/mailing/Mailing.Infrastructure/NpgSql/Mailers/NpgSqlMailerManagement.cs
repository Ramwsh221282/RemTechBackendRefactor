using Mailing.Core.Mailers;

namespace Mailing.Infrastructure.NpgSql.Mailers;

public static class NpgSqlMailerManagement
{
    extension(Mailer mailer)
    {
        public CommandDefinition MakeCommand(string sql, NpgSqlSession session, CancellationToken ct)
        {
            DynamicParameters parameters = mailer.FillParameters();
            CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
            return command;
        }
        
        private DynamicParameters FillParameters()
        {
            DynamicParameters parameters = new();
            parameters.Add("@id", mailer.Id);
            parameters.Add("@hashed_password", mailer.Config.SmtpPassword);
            parameters.Add("@service", mailer.Domain.Service);
            parameters.Add("@email", mailer.Domain.Email.Value);
            parameters.Add("@send_limit", mailer.Domain.SendLimit);
            parameters.Add("@send_current", mailer.Domain.CurrentSend);
            return parameters;
        }
    }
}