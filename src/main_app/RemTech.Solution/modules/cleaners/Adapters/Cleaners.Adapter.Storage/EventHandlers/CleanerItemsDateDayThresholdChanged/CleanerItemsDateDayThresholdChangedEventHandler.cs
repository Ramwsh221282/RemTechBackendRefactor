using Cleaners.Adapter.Storage.Common;
using Cleaners.Domain.Cleaners.Ports.Cache;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using Serilog;

namespace Cleaners.Adapter.Storage.EventHandlers.CleanerItemsDateDayThresholdChanged;

public sealed class CleanerItemsDateDayThresholdChangedEventHandler(
    CleanersDbContext context,
    ILogger logger,
    ICleanersCachedStorage cached
) : IDomainEventHandler<Domain.Cleaners.Events.CleanerItemsDateDayThresholdChanged>
{
    public async Task<Status> Handle(
        Domain.Cleaners.Events.CleanerItemsDateDayThresholdChanged @event,
        CancellationToken ct = default
    )
    {
        logger.Information("Handling cleaners event. Database level.");
        var cleaner = await context.GetLockedCleaner(@event.Id, ct);
        if (cleaner.IsFailure)
            return cleaner;

        cleaner.Value.ItemsDateDayThreshold = @event.Threshold;

        var result = await cleaner.Value.Save(
            "Не удается сохранить новое значение для порога подозрительных объявлений.",
            logger,
            ct
        );

        if (result.IsFailure)
            return result;

        await cached.Invalidate(cleaner.Value.ConvertToDomainModel());
        return result;
    }
}
