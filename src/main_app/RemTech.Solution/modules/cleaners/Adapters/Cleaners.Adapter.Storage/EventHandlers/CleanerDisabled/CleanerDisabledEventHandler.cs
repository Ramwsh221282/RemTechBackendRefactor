using Cleaners.Adapter.Storage.Common;
using Cleaners.Domain.Cleaners.Ports.Cache;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using Serilog;

namespace Cleaners.Adapter.Storage.EventHandlers.CleanerDisabled;

public sealed class CleanerDisabledEventHandler(
    CleanersDbContext context,
    ILogger logger,
    ICleanersCachedStorage cached
) : IDomainEventHandler<Domain.Cleaners.Events.CleanerDisabled>
{
    public async Task<Status> Handle(
        Domain.Cleaners.Events.CleanerDisabled @event,
        CancellationToken ct = default
    )
    {
        logger.Information("Handling cleaners event. Database level.");
        var cleaner = await context.GetLockedCleaner(@event.Id, ct);
        if (cleaner.IsFailure)
            return Status.Failure(cleaner.Error);

        cleaner.Value.State = @event.State;

        var result = await cleaner.Value.Save(
            "Не удается сохранить состояние чистильщика в базе данных.",
            logger,
            ct
        );

        if (result.IsFailure)
            return result;

        await cached.Invalidate(cleaner.Value.ConvertToDomainModel());
        return result;
    }
}
