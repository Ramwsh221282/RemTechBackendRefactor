using ParsersControl.Core.Features.PermantlyStartManyParsing;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.PermantlyStartManyParsing;

public sealed class PermantlyStartManyParsingCacheInvalidator(CachedParserArrayInvalidator invalidator)
    : ICacheInvalidator<PermantlyStartManyParsingCommand, IEnumerable<SubscribedParser>>
{
    public async Task InvalidateCache(
        PermantlyStartManyParsingCommand command, 
        IEnumerable<SubscribedParser> result,
        CancellationToken ct = default) =>
        await invalidator.Invalidate(ct);    
}