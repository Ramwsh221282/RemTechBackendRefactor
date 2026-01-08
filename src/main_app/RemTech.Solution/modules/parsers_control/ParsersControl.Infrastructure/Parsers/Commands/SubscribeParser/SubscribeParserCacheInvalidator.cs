using ParsersControl.Core.Features.SubscribeParser;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.SubscribeParser;

public sealed class SubscribeParserCacheInvalidator(CachedParserArrayInvalidator invalidator) : ICacheInvalidator<SubscribeParserCommand, SubscribedParser>
{
    public async Task InvalidateCache(
        SubscribeParserCommand command, 
        SubscribedParser result,
        CancellationToken ct = default) =>
        await invalidator.Invalidate(ct);
}