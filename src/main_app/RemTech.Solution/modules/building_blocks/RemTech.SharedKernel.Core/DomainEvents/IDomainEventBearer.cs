namespace RemTech.SharedKernel.Core.DomainEvents;

/// <summary>
/// Интерфейс носителя доменных событий.
/// </summary>
public interface IDomainEventBearer
{
	/// <summary>
	/// Коллекция доменных событий.
	/// </summary>
	IReadOnlyList<IDomainEvent> Events { get; }
}
