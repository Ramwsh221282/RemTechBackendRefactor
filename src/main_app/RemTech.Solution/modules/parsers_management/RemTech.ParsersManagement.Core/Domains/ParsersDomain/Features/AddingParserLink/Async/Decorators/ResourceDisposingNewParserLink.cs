using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingParserLink.Async.Decorators;

public sealed class ResourceDisposingNewParserLink(IAsyncNewParserLink origin) : IAsyncNewParserLink
{
    private readonly List<IDisposable> _disposables = [];

    public async Task<Status<IParserLink>> AsyncNew(
        AsyncAddParserLink add,
        CancellationToken ct = default
    )
    {
        Status<IParserLink> status = await origin.AsyncNew(add, ct);
        foreach (var disposable in _disposables)
            disposable.Dispose();
        return status;
    }

    public ResourceDisposingNewParserLink With(IDisposable disposable)
    {
        _disposables.Add(disposable);
        return this;
    }
}
