using Cleaners.Adapter.Storage.Common;
using Cleaners.Domain.Cleaners.Ports;
using Cleaners.Domain.Cleaners.Ports.Cache;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using Serilog;

namespace Cleaners.Adapter.Storage.EventHandlers.CleanerCleanedItem;

public sealed class CleanerCleanedItemEventHandler(
    CleanersDbContext context,
    ILogger logger,
    ICleanersCachedStorage cached
) : IDomainEventHandler<Domain.Cleaners.Events.CleanerCleanedItem>
{
    public async Task<Status> Handle(
        Domain.Cleaners.Events.CleanerCleanedItem @event,
        CancellationToken ct = default
    )
    {
        logger.Information("Handling cleaners event. Database level.");
        var cleaner = await context.GetLockedCleaner(@event.Id, ct: ct);
        if (cleaner.IsFailure)
            return Status.Failure(cleaner.Error);

        cleaner.Value.ItemsDateDayThreshold = @event.Threshold;

        var saving = await cleaner.Value.Save(
            "Не удается обновить количество очищенных записей у чистильщика",
            logger,
            ct
        );

        if (saving.IsFailure)
            return saving;

        await cached.Invalidate(cleaner.Value.ConvertToDomainModel());
        return saving;
    }
}
