namespace RemTech.SharedKernel.Core.Handlers;

public interface IDomainEventHandler
{
    Task Handle(IDomainEvent @event, CancellationToken ct = default);
}