using Cleaners.Adapter.Storage.Common;
using Cleaners.Domain.Cleaners.Events;
using Cleaners.Domain.Cleaners.Ports.Cache;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Cleaners.Adapter.Storage.EventHandlers.ScheduleUpdated;

public sealed class CleanerScheduleUpdatedEventHandler(
    CleanersDbContext context,
    Serilog.ILogger logger,
    ICleanersCachedStorage cached
) : IDomainEventHandler<CleanerScheduleUpdated>
{
    public async Task<Status> Handle(CleanerScheduleUpdated @event, CancellationToken ct = default)
    {
        var cleaner = await context.GetLockedCleaner(@event.Id, ct: ct);
        if (cleaner.IsFailure)
            return cleaner;

        cleaner.Value.LastRun = @event.LastRun;
        cleaner.Value.NextRun = @event.NextRun;
        cleaner.Value.WaitDays = @event.WaitDays;

        var saving = await cleaner.Value.Save(
            "Не удается сохранить новое расписание чистильщика",
            logger,
            ct
        );
        if (saving.IsFailure)
            return saving;

        await cached.Invalidate(cleaner.Value.ConvertToDomainModel());
        return saving;
    }
}
