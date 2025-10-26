using Cleaners.Adapter.Storage.Common;
using Cleaners.Domain.Cleaners.Ports;
using Cleaners.Domain.Cleaners.Ports.Cache;
using RemTech.Core.Shared.Result;
using Serilog;

namespace Cleaners.Adapter.Storage.EventHandlers.CleanerWorkTimeChanged;

public sealed class CleanerWorkTimeChangedEventHandler(
    CleanersDbContext context,
    ILogger logger,
    ICleanersCachedStorage cached
)
{
    public async Task<Status> Handle(
        Domain.Cleaners.Events.CleanerWorkTimeChanged @event,
        CancellationToken ct = default
    )
    {
        logger.Information("Handling cleaners event. Database level.");
        var cleaner = await context.GetLockedCleaner(@event.Id, ct);
        if (cleaner.IsFailure)
            return cleaner;

        cleaner.Value.Hours = @event.ElapsedHours;
        cleaner.Value.Seconds = @event.ElapsedSeconds;
        cleaner.Value.Minutes = @event.ElapsedMinutes;

        var result = await cleaner.Value.Save(
            "Не удается сохранить статистику работы чистильщика по времени",
            logger,
            ct
        );

        if (result.IsFailure)
            return result;

        await cached.Invalidate(cleaner.Value.ConvertToDomainModel());
        return result;
    }
}
