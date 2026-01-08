using ParsersControl.Core.Features.SetWorkTime;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.SetWorkTime;

public sealed class SetWorkTimeCacheInvalidator(CachedParserArrayInvalidator invalidator)
    : ICacheInvalidator<SetWorkTimeCommand, SubscribedParser>
{
    public async Task InvalidateCache(
        SetWorkTimeCommand command, 
        SubscribedParser result,
        CancellationToken ct = default) =>
        await invalidator.Invalidate(ct);    
}