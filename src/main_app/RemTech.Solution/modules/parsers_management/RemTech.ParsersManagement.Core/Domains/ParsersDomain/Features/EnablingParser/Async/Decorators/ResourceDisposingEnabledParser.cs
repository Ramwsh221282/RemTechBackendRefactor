using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.EnablingParser.Async.Decorators;

public sealed class ResourceDisposingEnabledParser(IAsyncEnabledParser origin) : IAsyncEnabledParser
{
    private readonly List<IDisposable> _disposables = [];

    public async Task<Status<IParser>> EnableAsync(
        AsyncEnableParser enable,
        CancellationToken ct = default
    )
    {
        Status<IParser> status = await origin.EnableAsync(enable, ct);
        foreach (IDisposable disposable in _disposables)
            disposable.Dispose();
        return status;
    }

    public ResourceDisposingEnabledParser With(IDisposable disposable)
    {
        _disposables.Add(disposable);
        return this;
    }
}
