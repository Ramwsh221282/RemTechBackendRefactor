using ParsersControl.Core.Features.StartParserWork;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.StartParserWork;

public sealed class StartParserCacheInvalidator(CachedParserArrayInvalidator invalidator)
    : ICacheInvalidator<StartParserCommand, SubscribedParser>
{
    public async Task InvalidateCache(
        StartParserCommand command, 
        SubscribedParser result,
        CancellationToken ct = default) =>
        await invalidator.Invalidate(ct);    
}