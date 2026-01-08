using ParsersControl.Core.Features.PermantlyDisableManyParsing;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.PermantlyDisableManyParsing;

public sealed class PermantlyDisableManyParsingCacheInvalidator(CachedParserArrayInvalidator invalidator)
    : ICacheInvalidator<PermantlyDisableManyParsingCommand, IEnumerable<SubscribedParser>>
{
    public async Task InvalidateCache(
        PermantlyDisableManyParsingCommand command, 
        IEnumerable<SubscribedParser> result,
        CancellationToken ct = default) =>
        await invalidator.Invalidate(ct);    
}