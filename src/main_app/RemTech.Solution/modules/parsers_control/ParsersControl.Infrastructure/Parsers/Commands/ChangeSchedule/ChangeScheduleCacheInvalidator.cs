using ParsersControl.Core.Features.ChangeSchedule;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.ChangeSchedule;

public sealed class ChangeScheduleCacheInvalidator(CachedParserArrayInvalidator invalidator)
    : ICacheInvalidator<ChangeScheduleCommand, SubscribedParser>
{
    public async Task InvalidateCache(
        ChangeScheduleCommand command, 
        SubscribedParser result,
        CancellationToken ct = default) =>
        await invalidator.Invalidate(ct);
}