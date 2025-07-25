using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StoppedParser.Async.Decorators;

public sealed class ResourceDisposingStoppedParser(IAsyncStoppedParser origin) : IAsyncStoppedParser
{
    private readonly List<IDisposable> _disposables = [];

    public async Task<Status<IParser>> AsyncStopped(
        AsyncStopParser stop,
        CancellationToken ct = default
    )
    {
        Status<IParser> status = await origin.AsyncStopped(stop, ct);
        foreach (IDisposable disposable in _disposables)
            disposable.Dispose();
        return status;
    }

    public ResourceDisposingStoppedParser With(IDisposable disposable)
    {
        _disposables.Add(disposable);
        return this;
    }
}
