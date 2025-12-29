using Mailing.Core.Mailers;
using Mailing.Core.Mailers.Protocols;

namespace Mailing.Infrastructure.NpgSql.Mailers;

public sealed record NpgSqlSaveMailerProtocol(NpgSqlSession session) : SaveMailerProtocol
{
    public async Task Save(Mailer mailer, CancellationToken ct)
    {
        const string sql =
            """
            UPDATE mailing_module.mailers SET
            hashed_password = @hashed_password,
            email = @email,
            send_limit = @send_limit,
            send_current = @send_current
            """;
        await session.Execute(mailer.MakeCommand(sql, session, ct));
    }
}