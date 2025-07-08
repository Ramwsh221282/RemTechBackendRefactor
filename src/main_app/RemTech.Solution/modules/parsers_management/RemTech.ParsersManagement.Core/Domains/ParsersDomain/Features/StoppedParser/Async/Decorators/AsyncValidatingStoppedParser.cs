using RemTech.Core.Shared.Functional;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StoppedParser.Async.Decorators;

public sealed class AsyncValidatingStoppedParser(IAsyncStoppedParser inner) : IAsyncStoppedParser
{
    public Task<Status<IParser>> AsyncStopped(
        AsyncStopParser stop,
        CancellationToken ct = default
    ) => new AsyncValidatingOperation(stop).Process(inner.AsyncStopped(stop, ct));
}
