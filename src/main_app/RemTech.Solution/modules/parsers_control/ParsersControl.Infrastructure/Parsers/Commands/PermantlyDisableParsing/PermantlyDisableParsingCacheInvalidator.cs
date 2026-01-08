using ParsersControl.Core.Features.PermantlyDisableParsing;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.PermantlyDisableParsing;

public sealed class PermantlyDisableParsingCacheInvalidator(CachedParserArrayInvalidator invalidator)
    : ICacheInvalidator<PermantlyDisableParsingCommand, SubscribedParser>
{
    public async Task InvalidateCache(
        PermantlyDisableParsingCommand command, 
        SubscribedParser result,
        CancellationToken ct = default) =>
        await invalidator.Invalidate(ct);    
}