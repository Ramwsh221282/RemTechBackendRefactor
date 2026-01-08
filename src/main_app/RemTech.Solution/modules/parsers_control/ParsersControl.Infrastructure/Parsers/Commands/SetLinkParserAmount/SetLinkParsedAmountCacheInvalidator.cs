using ParsersControl.Core.Features.SetLinkParsedAmount;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.SetLinkParserAmount;

public sealed class SetLinkParsedAmountCacheInvalidator(CachedParserArrayInvalidator invalidator)
    : ICacheInvalidator<SetLinkParsedAmountCommand, SubscribedParserLink>
{
    public async Task InvalidateCache(
        SetLinkParsedAmountCommand command, 
        SubscribedParserLink result,
        CancellationToken ct = default) =>
        await invalidator.Invalidate(ct);
}