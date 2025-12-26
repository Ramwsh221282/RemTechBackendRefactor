using Cleaners.Domain.Cleaners.Aggregate;
using RemTech.Core.Shared.DomainEvents;

namespace Cleaners.Domain.Cleaners.Events;

public sealed record CleanerItemsDateDayThresholdChanged(Guid Id, int Threshold) : IDomainEvent
{
    public CleanerItemsDateDayThresholdChanged(Cleaner cleaner)
        : this(cleaner.Id, cleaner.ItemsDateDayThreshold) { }
}
