using Cleaners.Domain.Cleaners.Aggregate;
using RemTech.Core.Shared.DomainEvents;

namespace Cleaners.Domain.Cleaners.Events;

public sealed record CleanerStartedWaiting(Guid Id, string State) : IDomainEvent
{
    public CleanerStartedWaiting(Cleaner cleaner)
        : this(cleaner.Id, cleaner.State) { }
}
