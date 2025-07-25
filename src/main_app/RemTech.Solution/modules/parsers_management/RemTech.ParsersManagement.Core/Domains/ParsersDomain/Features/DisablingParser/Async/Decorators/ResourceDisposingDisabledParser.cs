using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.DisablingParser.Async.Decorators;

public sealed class ResourceDisposingDisabledParser(IAsyncDisabledParser origin)
    : IAsyncDisabledParser
{
    private readonly List<IDisposable> _disposables = [];

    public async Task<Status<IParser>> Disable(
        AsyncDisableParser disable,
        CancellationToken ct = default
    )
    {
        Status<IParser> status = await origin.Disable(disable, ct);
        foreach (IDisposable disposable in _disposables)
            disposable.Dispose();
        return status;
    }

    public ResourceDisposingDisabledParser With(IDisposable disposable)
    {
        _disposables.Add(disposable);
        return this;
    }
}
