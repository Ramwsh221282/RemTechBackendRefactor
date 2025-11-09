using Mailers.Core.MailersContext;
using Mailers.Persistence.NpgSql;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Application.Features;

public sealed record InsertMailerSendingFunctionArguments(MailerSending Sending, NpgSqlSession Session) : IFunctionArgument;

public sealed class InsertMailerSendingFunction : IAsyncFunction<InsertMailerSendingFunctionArguments, Result<MailerSending>>
{
    public async Task<Result<MailerSending>> Invoke(InsertMailerSendingFunctionArguments argument, CancellationToken ct)
    {
        var session = argument.Session;
        var sending = argument.Sending;
        await sending.Insert(session, ct);
        await sending.Mailer.Update(session, ct);
        return argument.Sending;
    }
}