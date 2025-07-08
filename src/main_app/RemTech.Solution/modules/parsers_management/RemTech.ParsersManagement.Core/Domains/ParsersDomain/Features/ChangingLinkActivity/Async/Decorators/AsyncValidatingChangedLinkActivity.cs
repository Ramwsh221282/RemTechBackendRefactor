using RemTech.Core.Shared.Functional;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.ChangingLinkActivity.Async.Decorators;

public sealed class AsyncValidatingChangedLinkActivity(IAsyncChangedLinkActivity inner)
    : IAsyncChangedLinkActivity
{
    public Task<Status<IParserLink>> AsyncChangedActivity(
        AsyncChangeLinkActivity change,
        CancellationToken ct = default
    ) => new AsyncValidatingOperation(change).Process(inner.AsyncChangedActivity(change, ct));
}
