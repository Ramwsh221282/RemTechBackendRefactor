using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.RemovingParserLink.Async;

public sealed class AsyncRemovedParserLink(IRemovedParserLink inner) : IAsyncRemovedParserLink
{
    public Task<Status<IParserLink>> AsyncRemoved(
        AsyncRemoveParserLink remove,
        CancellationToken ct = default
    )
    {
        return Task.FromResult(
            inner.Removed(new RemoveParserLink(remove.Take(), remove.WhomRemoveId()))
        );
    }
}