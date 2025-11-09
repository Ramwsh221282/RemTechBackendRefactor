using Mailers.Core.MailersContext;
using Mailers.Persistence.NpgSql;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Application.Features;

public sealed record InsertMailerFunctionArgument(NpgSqlSession Session, Mailer Mailer) : IFunctionArgument;

public sealed class InsertMailerFunction : IAsyncFunction<InsertMailerFunctionArgument, Result<Unit>>
{
    public async Task<Result<Unit>> Invoke(InsertMailerFunctionArgument argument, CancellationToken ct)
    {
        var session = argument.Session;
        var mailer = argument.Mailer;
        
        if (!await mailer.HasUniqueEmail(session, ct))
            return Conflict($"Почтовый отправитель с почтой: {mailer.Metadata.Email} уже существует.");
        
        await mailer.Insert(session, ct);
        return Unit.Value;
    }
}