using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

namespace RemTech.SharedKernel.Core.DomainEvents;

public interface IDomainEvent
{
	public Task PublishTo(IDomainEventHandler handler, CancellationToken ct = default);
}
