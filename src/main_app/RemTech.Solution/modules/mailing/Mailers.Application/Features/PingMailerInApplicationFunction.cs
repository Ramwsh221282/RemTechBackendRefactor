using Mailers.Core.MailersContext;
using Mailers.Persistence.NpgSql;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Application.Features;

public sealed record PingMailerInApplicationFunctionArgument(Guid Id, string To, NpgSqlSession Session) : IFunctionArgument;

public sealed class PingMailerInApplicationFunction : IAsyncFunction<PingMailerInApplicationFunctionArgument, Result<Mailer>>
{
    private readonly IFunction<PingMailerSenderFunctionArgument, Result<MailerSending>> _ping;
    private readonly IAsyncFunction<InsertMailerSendingFunctionArguments, Result<MailerSending>> _insert;

    public PingMailerInApplicationFunction(
        IFunction<PingMailerSenderFunctionArgument, Result<MailerSending>> ping,
        IAsyncFunction<InsertMailerSendingFunctionArguments, Result<MailerSending>> insert
        )
    {
        _ping = ping;
        _insert = insert;
    }
    
    
    public async Task<Result<Mailer>> Invoke(PingMailerInApplicationFunctionArgument argument, CancellationToken ct)
    {
        var session = argument.Session;
        await session.GetTransaction(ct);
        var mailer = await new QueryMailerArguments(Id: argument.Id).Get(session, ct, true);
        if (mailer.NoValue) return NotFound("Почтовый отправитель не найден.");
        var pinging = _ping.Invoke(new PingMailerSenderFunctionArgument(mailer.Value, argument.To));
        if (pinging.IsFailure) return pinging.Error;
        var inserting = await _insert.Invoke(new InsertMailerSendingFunctionArguments(pinging, session), ct);
        if (inserting.IsFailure) return inserting.Error;
        if (!await session.Commited(ct))
            return Error.Application("Не удается зафиксировать создание нового отправленного сообщения.");
        return pinging.Value.Mailer;
    }
}