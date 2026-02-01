using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

namespace RemTech.SharedKernel.Core.DomainEvents;

/// <summary>
/// Диспетчер доменных событий.
/// </summary>
/// <param name="handlers">Коллекция обработчиков доменных событий.</param>
public sealed class DomainEventsDispatcher(IEnumerable<IDomainEventHandler> handlers)
{
	/// <summary>
	/// Диспетчеризация доменных событий из носителя событий.
	/// </summary>
	/// <param name="bearer">Носитель доменных событий.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию.</returns>
	public Task Dispatch(IDomainEventBearer bearer, CancellationToken ct = default)
	{
		return Dispatch(bearer.Events, ct);
	}

	/// <summary>
	/// Диспетчеризация доменного события.
	/// </summary>
	/// <param name="event">Доменное событие.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию.</returns>
	public async Task Dispatch(IDomainEvent @event, CancellationToken ct = default)
	{
		foreach (IDomainEventHandler handler in handlers)
		{
			await handler.Handle(@event, ct);
		}
	}

	/// <summary>
	/// Диспетчеризация коллекции доменных событий.
	/// </summary>
	/// <param name="events">Коллекция доменных событий.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию.</returns>
	public async Task Dispatch(IEnumerable<IDomainEvent> events, CancellationToken ct = default)
	{
		foreach (IDomainEvent @event in events)
		{
			await Dispatch(@event, ct);
		}
	}
}
