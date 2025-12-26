using Cleaners.Domain.Cleaners.Aggregate;
using RemTech.Core.Shared.DomainEvents;

namespace Cleaners.Domain.Cleaners.Events;

public sealed record CleanerCreatedEvent(
    Guid Id,
    int CleanedAmount,
    string State,
    DateTime? LastRun,
    DateTime? NextRun,
    int WaitDays,
    int ElapsedSeconds,
    int ElapsedMinutes,
    int ElapsedHours,
    int ItemsDateDayThreshold
) : IDomainEvent
{
    public CleanerCreatedEvent(Cleaner cleaner)
        : this(
            cleaner.Id,
            cleaner.CleanedAmount,
            cleaner.State,
            cleaner.Schedule.LastRun,
            cleaner.Schedule.NextRun,
            cleaner.Schedule.WaitDays,
            cleaner.WorkTime.ElapsedSeconds,
            cleaner.WorkTime.ElapsedMinutes,
            cleaner.WorkTime.ElapsedHours,
            cleaner.ItemsDateDayThreshold
        ) { }
}
