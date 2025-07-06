using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.RemovingParserLink.Async.Decorators;

public sealed class AsyncValidatingRemovedParserLink(IAsyncRemovedParserLink inner)
    : IAsyncRemovedParserLink
{
    public Task<Status<IParserLink>> AsyncRemoved(
        AsyncRemoveParserLink remove,
        CancellationToken ct = default
    ) =>
        remove.Errored()
            ? Task.FromResult(Status<IParserLink>.Failure(remove.Error()))
            : inner.AsyncRemoved(remove, ct);
}