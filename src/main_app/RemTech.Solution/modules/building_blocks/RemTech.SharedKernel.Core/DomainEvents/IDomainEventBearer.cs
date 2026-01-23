namespace RemTech.SharedKernel.Core.DomainEvents;

public interface IDomainEventBearer
{
	public IReadOnlyList<IDomainEvent> Events { get; }
}
