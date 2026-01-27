namespace RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

/// <summary>
/// Интерфейс инвалидатора кэша.
/// </summary>
/// <typeparam name="TCommand">Тип команды.</typeparam>
/// <typeparam name="TResult">Тип результата.</typeparam>
public interface ICacheInvalidator<in TCommand, in TResult>
	where TCommand : ICommand
{
	/// <summary>
	/// Инвалидирует кэш на основе команды и результата.
	/// </summary>
	/// <param name="command">Команда.</param>
	/// <param name="result">Результат выполнения команды.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию инвалидирования кэша.</returns>
	Task InvalidateCache(TCommand command, TResult result, CancellationToken ct = default);
}
