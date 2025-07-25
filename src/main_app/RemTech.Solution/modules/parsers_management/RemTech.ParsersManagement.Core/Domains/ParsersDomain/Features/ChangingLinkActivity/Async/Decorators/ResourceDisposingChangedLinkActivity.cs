using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.ChangingLinkActivity.Async.Decorators;

public sealed class ResourceDisposingChangedLinkActivity(IAsyncChangedLinkActivity origin)
    : IAsyncChangedLinkActivity
{
    private readonly List<IDisposable> _disposables = [];

    public async Task<Status<IParserLink>> AsyncChangedActivity(
        AsyncChangeLinkActivity change,
        CancellationToken ct = default
    )
    {
        Status<IParserLink> status = await origin.AsyncChangedActivity(change, ct);
        foreach (IDisposable disposable in _disposables)
            disposable.Dispose();
        return status;
    }

    public ResourceDisposingChangedLinkActivity With(IDisposable disposable)
    {
        _disposables.Add(disposable);
        return this;
    }
}
