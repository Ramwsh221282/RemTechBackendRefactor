using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

namespace RemTech.SharedKernel.Core.DomainEvents;

/// <summary>
/// Интерфейс доменного события.
/// </summary>
public interface IDomainEvent
{
	/// <summary>
	/// Публикация доменного события указанному обработчику.
	/// </summary>
	/// <param name="handler">Обработчик доменного события.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию.</returns>
	Task PublishTo(IDomainEventHandler handler, CancellationToken ct = default);
}
