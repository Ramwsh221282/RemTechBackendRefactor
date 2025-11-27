using Dapper;
using Mailing.Core.Mailers;
using Mailing.Core.Mailers.Protocols;
using RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace Mailing.Infrastructure.NpgSql.Mailers;

public sealed record NpgSqlSaveMailerProtocol(NpgSqlSession session) : SaveMailerProtocol
{
    public async Task Save(Mailer mailer, CancellationToken ct)
    {
        if (!await IsEmailUnique(mailer, ct))
            throw ErrorException.Conflict($"Конфигурация почтового сервиса с почтой: {mailer.Domain.Email.Value} уже существует.");
            
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

    private async Task<bool> IsEmailUnique(Mailer mailer, CancellationToken ct)
    {
        const string sql = """
                           SELECT EXISTS(SELECT 1 FROM mailing_module.mailers WHERE email = @email)
                           """;
        CommandDefinition command = mailer.MakeCommand(sql, session, ct);
        bool exists = await session.QuerySingleRow<bool>(command);
        return !exists;
    }
}