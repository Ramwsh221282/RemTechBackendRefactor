using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

namespace RemTech.SharedKernel.Core.DomainEvents;

public interface IDomainEvent
{
    Task PublishTo(IDomainEventHandler handler, CancellationToken ct = default);
}
