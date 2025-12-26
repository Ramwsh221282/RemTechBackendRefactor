using Cleaners.Domain.Cleaners.Aggregate;
using RemTech.Core.Shared.DomainEvents;

namespace Cleaners.Domain.Cleaners.Events;

public sealed record CleanerCleanedItem(Guid Id, int CleanedItemsCount) : IDomainEvent
{
    public CleanerCleanedItem(Cleaner cleaner)
        : this(cleaner.Id, cleaner.CleanedAmount) { }
}
