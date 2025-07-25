using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.FinishingParserLink.Async.Decorators;

public sealed class ResourceDisposingFinishedParserLink(IAsyncFinishedParserLink origin)
    : IAsyncFinishedParserLink
{
    private readonly List<IDisposable> _disposables = [];

    public async Task<Status<IParserLink>> AsyncFinished(
        AsyncFinishParserLink finish,
        CancellationToken ct = default
    )
    {
        Status<IParserLink> status = await origin.AsyncFinished(finish, ct);
        foreach (IDisposable disposable in _disposables)
            disposable.Dispose();
        return status;
    }

    public ResourceDisposingFinishedParserLink With(IDisposable disposable)
    {
        _disposables.Add(disposable);
        return this;
    }
}
