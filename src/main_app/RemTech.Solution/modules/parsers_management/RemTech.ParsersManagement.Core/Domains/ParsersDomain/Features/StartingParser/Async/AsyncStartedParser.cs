using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StartingParser.Async;

public sealed class AsyncStartedParser(IStartedParser inner) : IAsyncStartedParser
{
    public Task<Status<IParser>> StartedAsync(
        AsyncStartParser start,
        CancellationToken ct = default
    )
    {
        return Task.FromResult(inner.Started(new StartParser(start.Take())));
    }
}