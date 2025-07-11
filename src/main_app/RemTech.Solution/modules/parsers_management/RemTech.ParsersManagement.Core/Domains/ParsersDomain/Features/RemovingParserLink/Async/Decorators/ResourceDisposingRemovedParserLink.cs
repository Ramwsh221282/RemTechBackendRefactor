using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.RemovingParserLink.Async.Decorators;

public sealed class ResourceDisposingRemovedParserLink(IAsyncRemovedParserLink origin)
    : IAsyncRemovedParserLink
{
    private readonly List<IDisposable> _disposables = [];

    public async Task<Status<IParserLink>> AsyncRemoved(
        AsyncRemoveParserLink remove,
        CancellationToken ct = default
    )
    {
        Status<IParserLink> status = await origin.AsyncRemoved(remove, ct);
        foreach (IDisposable disposable in _disposables)
            disposable.Dispose();
        return status;
    }

    public ResourceDisposingRemovedParserLink With(IDisposable disposable)
    {
        _disposables.Add(disposable);
        return this;
    }
}
