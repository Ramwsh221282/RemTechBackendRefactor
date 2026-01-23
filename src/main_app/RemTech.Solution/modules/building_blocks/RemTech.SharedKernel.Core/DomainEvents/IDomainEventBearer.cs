namespace RemTech.SharedKernel.Core.DomainEvents;

public interface IDomainEventBearer
{
	IReadOnlyList<IDomainEvent> Events { get; }
}
