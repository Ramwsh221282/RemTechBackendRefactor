namespace RemTech.SharedKernel.Core.Handlers;

public interface IDomainEvent
{
    Task PublishTo(IDomainEventHandler handler, CancellationToken ct = default);
}

public interface IDomainEventHandler<TEvent> where TEvent : IDomainEvent
{
    Task Handle(TEvent @event, CancellationToken ct = default);
}