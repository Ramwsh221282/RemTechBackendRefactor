using Cleaners.Domain.Cleaners.Aggregate;
using RemTech.Core.Shared.DomainEvents;

namespace Cleaners.Domain.Cleaners.Events;

public sealed record CleanerWorkTimeChanged(
    Guid Id,
    int ElapsedSeconds,
    int ElapsedMinutes,
    int ElapsedHours
) : IDomainEvent
{
    public CleanerWorkTimeChanged(Cleaner cleaner)
        : this(
            cleaner.Id,
            cleaner.WorkTime.ElapsedSeconds,
            cleaner.WorkTime.ElapsedMinutes,
            cleaner.WorkTime.ElapsedHours
        ) { }
}
