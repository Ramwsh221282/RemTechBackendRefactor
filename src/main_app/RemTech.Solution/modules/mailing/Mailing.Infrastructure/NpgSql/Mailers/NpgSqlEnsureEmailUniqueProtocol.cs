using Dapper;
using Mailing.Core.Mailers;
using Mailing.Core.Mailers.Protocols;
using RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace Mailing.Infrastructure.NpgSql.Mailers;

public sealed class NpgSqlEnsureEmailUniqueProtocol(NpgSqlSession session) : EnsureMailerEmailUniqueProtocol
{
    public async Task EnsureEmailUnique(Mailer mailer, CancellationToken ct)
    {
        const string sql = """
                           SELECT EXISTS(SELECT 1 FROM mailing_module.mailers WHERE email = @email)
                           """;
        CommandDefinition command = mailer.MakeCommand(sql, session, ct);
        bool exists = await session.QuerySingleRow<bool>(command);
        if (exists)
            throw ErrorException.Conflict($"Конфигурация почтового сервиса с почтой: {mailer.Domain.Email.Value} уже существует.");
    }
}