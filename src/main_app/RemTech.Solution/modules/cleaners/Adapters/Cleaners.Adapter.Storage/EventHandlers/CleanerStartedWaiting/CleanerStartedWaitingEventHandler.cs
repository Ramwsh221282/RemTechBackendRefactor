using Cleaners.Adapter.Storage.Common;
using Cleaners.Domain.Cleaners.Ports;
using Cleaners.Domain.Cleaners.Ports.Cache;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using Serilog;

namespace Cleaners.Adapter.Storage.EventHandlers.CleanerStartedWaiting;

public sealed class CleanerStartedWaitingEventHandler(
    CleanersDbContext context,
    ILogger logger,
    ICleanersCachedStorage cached
) : IDomainEventHandler<Domain.Cleaners.Events.CleanerStartedWaiting>
{
    public async Task<Status> Handle(
        Domain.Cleaners.Events.CleanerStartedWaiting @event,
        CancellationToken ct = default
    )
    {
        logger.Information("Handling cleaners event. Database level.");
        var cleaner = await context.GetLockedCleaner(@event.Id, ct);
        if (cleaner.IsFailure)
            return cleaner;

        cleaner.Value.State = @event.State;

        var result = await cleaner.Value.Save(
            "Не удается сохранить новое состояние чистильщика",
            logger,
            ct
        );

        if (result.IsFailure)
            return result;

        await cached.Invalidate(cleaner.Value.ConvertToDomainModel());
        return result;
    }
}
