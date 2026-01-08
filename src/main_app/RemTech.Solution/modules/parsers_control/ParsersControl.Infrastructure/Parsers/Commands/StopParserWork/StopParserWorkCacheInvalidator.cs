using ParsersControl.Core.Features.StopParserWork;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.StopParserWork;

public sealed class StopParserWorkCacheInvalidator(CachedParserArrayInvalidator invalidator)
    : ICacheInvalidator<StopParserWorkCommand, SubscribedParser>
{
    public async Task InvalidateCache(
        StopParserWorkCommand command, 
        SubscribedParser result,
        CancellationToken ct = default) =>
        await invalidator.Invalidate(ct);
}