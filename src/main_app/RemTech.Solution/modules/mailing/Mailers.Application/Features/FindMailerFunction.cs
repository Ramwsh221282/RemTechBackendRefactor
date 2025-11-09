using Mailers.Core.MailersContext;
using Mailers.Persistence.NpgSql;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Application.Features;

public sealed record FindMailerFunctionArgument(QueryMailerArguments Args, NpgSqlSession Session) : IFunctionArgument;

public sealed class FindMailerFunction : IAsyncFunction<FindMailerFunctionArgument, Optional<Mailer>>
{
    public async Task<Optional<Mailer>> Invoke(FindMailerFunctionArgument argument, CancellationToken ct)
    {
        var args = argument.Args;
        var session = argument.Session;
        return await args.Get(session, ct);
    }
}