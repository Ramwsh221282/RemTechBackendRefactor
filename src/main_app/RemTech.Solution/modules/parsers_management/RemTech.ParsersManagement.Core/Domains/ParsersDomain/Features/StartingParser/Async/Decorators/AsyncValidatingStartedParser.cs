using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StartingParser.Async.Decorators;

public sealed class AsyncValidatingStartedParser(IAsyncStartedParser inner) : IAsyncStartedParser
{
    public Task<Status<IParser>> StartedAsync(
        AsyncStartParser start,
        CancellationToken ct = default
    ) =>
        start.Errored()
            ? Task.FromResult(Status<IParser>.Failure(start.Error()))
            : inner.StartedAsync(start, ct);
}