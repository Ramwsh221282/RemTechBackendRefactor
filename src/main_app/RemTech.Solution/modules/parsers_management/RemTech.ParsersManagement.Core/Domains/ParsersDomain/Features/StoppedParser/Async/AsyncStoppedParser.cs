using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StoppedParser.Async;

public sealed class AsyncStoppedParser(IStoppedParser inner) : IAsyncStoppedParser
{
    public Task<Status<IParser>> AsyncStopped(AsyncStopParser stop, CancellationToken ct = default)
    {
        return Task.FromResult(inner.Stopped(new StopParser(stop.Take())));
    }
}
