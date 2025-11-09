using Mailers.Core.MailersContext;
using Mailers.Persistence.NpgSql;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Application.Features;

public sealed record DeleteMailerFunctionArgument(Mailer Mailer, NpgSqlSession Session) : IFunctionArgument;

public sealed class DeleteMailerFunction : IAsyncFunction<DeleteMailerFunctionArgument, Result<Unit>>
{
    public async Task<Result<Unit>> Invoke(DeleteMailerFunctionArgument argument, CancellationToken ct)
    {
        var session = argument.Session;
        var mailer = argument.Mailer;
        return await mailer.Delete(session, ct);
    }
}