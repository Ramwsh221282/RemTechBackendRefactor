using Cleaners.Domain.Cleaners.Aggregate;
using RemTech.Core.Shared.DomainEvents;

namespace Cleaners.Domain.Cleaners.Events;

public sealed record CleanerDisabled(Guid Id, string State) : IDomainEvent
{
    public CleanerDisabled(Cleaner cleaner)
        : this(cleaner.Id, cleaner.State) { }
}
