namespace RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

/// <summary>
/// Интерфейс обработчика команд с инвалидированием кэша.
/// </summary>
/// <typeparam name="TCommand">Тип команды.</typeparam>
/// <typeparam name="TResult">Тип результата.</typeparam>
public interface ICacheInvalidatingHandler<in TCommand, TResult> : ICommandHandler<TCommand, TResult>
	where TCommand : ICommand;
