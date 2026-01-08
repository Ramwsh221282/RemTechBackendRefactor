using ParsersControl.Core.Features.FinishParser;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.FinishParser;

public sealed class FinishParserCacheInvalidator(CachedParserArrayInvalidator invalidator) : ICacheInvalidator<FinishParserCommand, SubscribedParser>
{
    public async Task InvalidateCache(
        FinishParserCommand command, 
        SubscribedParser result,
        CancellationToken ct = default) =>
        await invalidator.Invalidate(ct);    
}