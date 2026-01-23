using RemTech.SharedKernel.Core.DomainEvents;

namespace RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

public interface IDomainEventHandler
{
	Task Handle(IDomainEvent @event, CancellationToken ct = default);
}

public interface IDomainEventHandler<in TEvent>
	where TEvent : IDomainEvent
{
	Task Handle(TEvent @event, CancellationToken ct = default);
}
