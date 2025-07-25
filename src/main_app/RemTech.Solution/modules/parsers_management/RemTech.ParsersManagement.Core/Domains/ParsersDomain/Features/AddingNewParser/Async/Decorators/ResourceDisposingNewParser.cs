using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingNewParser.Async.Decorators;

public sealed class ResourceDisposingNewParser(IAsyncNewParser origin) : IAsyncNewParser
{
    private readonly List<IDisposable> _disposables = [];

    public async Task<Status<IParser>> Register(
        AsyncAddNewParser add,
        CancellationToken ct = default
    )
    {
        Status<IParser> result = await origin.Register(add, ct);
        foreach (IDisposable disposable in _disposables)
            disposable.Dispose();
        return result;
    }

    public ResourceDisposingNewParser With(IDisposable disposable)
    {
        _disposables.Add(disposable);
        return this;
    }
}
