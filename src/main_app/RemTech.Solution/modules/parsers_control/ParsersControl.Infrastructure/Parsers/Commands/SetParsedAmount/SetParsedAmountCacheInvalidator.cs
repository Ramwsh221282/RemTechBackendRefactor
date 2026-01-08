using ParsersControl.Core.Features.SetParsedAmount;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.SetParsedAmount;

public sealed class SetParsedAmountCacheInvalidator(CachedParserArrayInvalidator invalidator)
    : ICacheInvalidator<SetParsedAmountCommand, SubscribedParser>
{
    public async Task InvalidateCache(
        SetParsedAmountCommand command, 
        SubscribedParser result,
        CancellationToken ct = default) =>
        await invalidator.Invalidate(ct);
}