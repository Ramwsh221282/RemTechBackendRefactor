using Cleaners.Domain.Cleaners.Aggregate;
using RemTech.Core.Shared.DomainEvents;

namespace Cleaners.Domain.Cleaners.Events;

public sealed record CleanerStatisticsUpdated(
    Guid Id,
    int Processed,
    int Hours,
    int Minutes,
    int Seocnds
) : IDomainEvent
{
    public CleanerStatisticsUpdated(Cleaner cleaner)
        : this(
            cleaner.Id,
            cleaner.CleanedAmount,
            cleaner.WorkTime.ElapsedHours,
            cleaner.WorkTime.ElapsedMinutes,
            cleaner.WorkTime.ElapsedSeconds
        ) { }
}
