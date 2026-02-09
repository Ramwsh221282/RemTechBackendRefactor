using RemTech.SharedKernel.Core.DomainEvents;

namespace RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

/// <summary>
/// Интерфейс обработчика доменных событий.
/// </summary>
public interface IDomainEventHandler
{
	/// <summary>
	/// Обрабатывает доменное событие.
	/// </summary>
	/// <param name="event">Доменное событие для обработки.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию обработки события.</returns>
	Task Handle(IDomainEvent @event, CancellationToken ct = default);
}

/// <summary>
/// Интерфейс обработчика доменных событий.
/// </summary>
/// <typeparam name="TEvent">Тип доменного события.</typeparam>
public interface IDomainEventHandler<in TEvent>
	where TEvent : IDomainEvent
{
	/// <summary>
	/// Обрабатывает доменное событие.
	/// </summary>
	/// <param name="event">Доменное событие для обработки.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию обработки события.</returns>
	Task Handle(TEvent @event, CancellationToken ct = default);
}
