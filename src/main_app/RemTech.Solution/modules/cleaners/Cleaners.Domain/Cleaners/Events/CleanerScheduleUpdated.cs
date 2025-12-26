using Cleaners.Domain.Cleaners.Aggregate;
using RemTech.Core.Shared.DomainEvents;

namespace Cleaners.Domain.Cleaners.Events;

public sealed record CleanerScheduleUpdated(
    Guid Id,
    DateTime? LastRun,
    DateTime? NextRun,
    int WaitDays
) : IDomainEvent
{
    public CleanerScheduleUpdated(Cleaner cleaner)
        : this(
            cleaner.Id,
            cleaner.Schedule.LastRun,
            cleaner.Schedule.NextRun,
            cleaner.Schedule.WaitDays
        ) { }
}
