using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser.Async.Decorators;

public sealed class ResourceDisposingUpdatedParser(IAsyncUpdatedParser origin) : IAsyncUpdatedParser
{
    private readonly List<IDisposable> _disposables = [];

    public async Task<Status<IParser>> Update(
        AsyncUpdateParser update,
        CancellationToken ct = default
    )
    {
        Status<IParser> status = await origin.Update(update, ct);
        foreach (IDisposable disposable in _disposables)
            disposable.Dispose();
        return status;
    }

    public ResourceDisposingUpdatedParser With(IDisposable disposable)
    {
        _disposables.Add(disposable);
        return this;
    }
}
