using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.ChangingLinkActivity.Async;

public sealed class AsyncChangedLinkActivity(IChangedLinkActivity inner) : IAsyncChangedLinkActivity
{
    public Task<Status<IParserLink>> AsyncChangedActivity(
        AsyncChangeLinkActivity change,
        CancellationToken ct = default
    ) =>
        Task.FromResult(
            inner.Changed(
                new ChangeLinkActivity(change.Take(), change.WhomChange(), change.NextActivity())
            )
        );
}
