using ParsersControl.Core.Features.PermantlyStartParsing;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.PermantlyStartParsing;

public sealed class PermantlyStartParsingCacheInvalidator(CachedParserArrayInvalidator invalidator)
    : ICacheInvalidator<PermantlyStartParsingCommand, SubscribedParser>
{
    public async Task InvalidateCache(
        PermantlyStartParsingCommand command, 
        SubscribedParser result,
        CancellationToken ct = default) =>
        await invalidator.Invalidate(ct);
}