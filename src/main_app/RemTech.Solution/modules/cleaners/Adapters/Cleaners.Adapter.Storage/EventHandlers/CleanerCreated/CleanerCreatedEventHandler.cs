using Cleaners.Adapter.Storage.Common;
using Cleaners.Adapter.Storage.DataModels;
using Cleaners.Domain.Cleaners.Events;
using Cleaners.Domain.Cleaners.Ports;
using Cleaners.Domain.Cleaners.Ports.Cache;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using Serilog;

namespace Cleaners.Adapter.Storage.EventHandlers.CleanerCreated;

public sealed class CleanerCreatedEventHandler(
    CleanersDbContext context,
    ILogger logger,
    ICleanersCachedStorage cached
) : IDomainEventHandler<CleanerCreatedEvent>
{
    private const string Context = nameof(CleanerCreatedEventHandler);

    public async Task<Status> Handle(CleanerCreatedEvent @event, CancellationToken ct = default)
    {
        logger.Information("Handling cleaners event. Database level.");
        var dataModel = CreateDataModel(@event);
        await context.Cleaners.AddAsync(dataModel, ct);

        try
        {
            await context.SaveChangesAsync(ct);
            await cached.Invalidate(dataModel.ConvertToDomainModel());
            return Status.Success();
        }
        catch (Exception ex)
        {
            logger.Error("{Context}. Error: {ErrorText}.", Context, ex);
            return Status.Internal("Не удается сохранить чистильщика в базе данных.");
        }
    }

    private CleanerDataModel CreateDataModel(CleanerCreatedEvent @event)
    {
        logger.Information("Handling cleaners event. Database level.");
        return new CleanerDataModel()
        {
            Id = @event.Id,
            State = @event.State,
            CleanedAmount = @event.CleanedAmount,
            ItemsDateDayThreshold = @event.ItemsDateDayThreshold,
            NextRun = @event.NextRun,
            LastRun = @event.LastRun,
            WaitDays = @event.WaitDays,
            Hours = @event.ElapsedHours,
            Minutes = @event.ElapsedMinutes,
            Seconds = @event.ElapsedSeconds,
        };
    }
}
