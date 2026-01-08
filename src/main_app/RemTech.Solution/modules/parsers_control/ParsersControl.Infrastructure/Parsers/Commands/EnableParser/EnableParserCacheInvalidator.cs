using ParsersControl.Core.Features.EnableParser;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.EnableParser;

public sealed class EnableParserCacheInvalidator(CachedParserArrayInvalidator invalidator) : ICacheInvalidator<EnableParserCommand, SubscribedParser>
{
    public async Task InvalidateCache(
        EnableParserCommand command, 
        SubscribedParser result,
        CancellationToken ct = default) =>
        await invalidator.Invalidate(ct);    
}