using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StartingParser.Async.Decorators;

public sealed class ResourceDisposingStartedParser(IAsyncStartedParser origin) : IAsyncStartedParser
{
    private readonly List<IDisposable> _disposables = [];

    public async Task<Status<IParser>> StartedAsync(
        AsyncStartParser start,
        CancellationToken ct = default
    )
    {
        Status<IParser> status = await origin.StartedAsync(start, ct);
        foreach (IDisposable disposable in _disposables)
            disposable.Dispose();
        return status;
    }

    public ResourceDisposingStartedParser With(IDisposable disposable)
    {
        _disposables.Add(disposable);
        return this;
    }
}
