namespace RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

/// <summary>
/// Интерфейс транспортировщика событий.
/// </summary>
/// <typeparam name="TCommand">Тип команды.</typeparam>
/// <typeparam name="TResult">Тип результата.</typeparam>
public interface IEventTransporter<in TCommand, in TResult>
	where TCommand : ICommand
{
	/// <summary>
	/// Транспортирует событие на основе результата.
	/// </summary>
	/// <param name="result">Результат, на основе которого транспортируется событие.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию транспортировки события.</returns>
	Task Transport(TResult result, CancellationToken ct = default);
}
