using Mailing.Core.Mailers;
using Mailing.Core.Mailers.Protocols;

namespace Mailing.Infrastructure.NpgSql.Mailers;

public sealed record NpgSqlPersistMailerProtocol(NpgSqlSession session) : PersistMailerProtocol
{
    public async Task Persist(Mailer mailer, CancellationToken ct)
    {
        const string sql =
            """
            INSERT INTO mailing_module.mailers
            (id, hashed_password, service, email, send_limit, send_current)
            VALUES
            (@id, @hashed_password, @service, @email, @send_limit, @send_current)
            """;
        CommandDefinition command = mailer.MakeCommand(sql, session, ct);
        await session.Execute(command);
    }
}