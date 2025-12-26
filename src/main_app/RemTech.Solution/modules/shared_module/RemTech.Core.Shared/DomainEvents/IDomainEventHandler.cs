using RemTech.Core.Shared.Result;

namespace RemTech.Core.Shared.DomainEvents;

public interface IDomainEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    Task<Status> Handle(TEvent @event, CancellationToken ct = default);
}
