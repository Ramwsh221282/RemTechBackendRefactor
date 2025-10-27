using Cleaners.Adapter.Storage.Common;
using Cleaners.Adapter.Storage.DataModels;
using Cleaners.Domain.Cleaners.Ports;
using Cleaners.Domain.Cleaners.Ports.Cache;
using Cleaners.Domain.Cleaners.Ports.Outbox;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using Serilog;

namespace Cleaners.Adapter.Storage.EventHandlers.CleanerWorkStarted;

public sealed class CleanerWorkStartedEventHandler(
    CleanersDbContext context,
    ILogger logger,
    ICleanersCachedStorage cached
) : IDomainEventHandler<Domain.Cleaners.Events.CleanerWorkStarted>
{
    public async Task<Status> Handle(
        Domain.Cleaners.Events.CleanerWorkStarted @event,
        CancellationToken ct = default
    )
    {
        logger.Information("Handling cleaners event. Database level.");
        await using var txn = await context.Database.BeginTransactionAsync(ct);

        try
        {
            var cleaner = await context.GetCleanerForUpdate(@event.Id, ct);
            if (cleaner.IsFailure)
                return cleaner;

            cleaner.Value.State = @event.State;
            cleaner.Value.CleanedAmount = @event.CleanedAmount;
            cleaner.Value.Hours = @event.ElapsedHours;
            cleaner.Value.Minutes = @event.ElapsedMinutes;
            cleaner.Value.Seconds = @event.ElapsedSeconds;
            cleaner.Value.LastRun = @event.LastRun;
            cleaner.Value.NextRun = @event.NextRun;
            cleaner.Value.WaitDays = @event.WaitDays;

            await context.SaveChangesAsync(ct);
            await cached.Invalidate(cleaner.Value.ConvertToDomainModel());

            var message = CleanerOutboxMessage.Create(cleaner.Value);
            await context.Outbox.AddAsync(message, ct);
            await context.SaveChangesAsync(ct);

            await txn.CommitAsync(ct);
            return Status.Success();
        }
        catch (Exception ex)
        {
            Status status = Status.Internal("Не удается сохранить новое состояние чистильщика");
            logger.Error(ex, ex.Message);
            await txn.RollbackAsync(ct);
            return status;
        }
    }
}
