namespace RemTech.SharedKernel.Core.Handlers.Decorators.Logging;

/// <summary>
/// Интерфейс логирующего обработчика команд (декоратор).
/// </summary>
/// <typeparam name="TCommand">Команда.</typeparam>
/// <typeparam name="TResult">Результат.</typeparam>
public interface ILoggingCommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
	where TCommand : ICommand;

/// <summary>
/// Интерфейс логирующего обработчика запросов (декоратор).
/// </summary>
/// <typeparam name="TQuery">Запрос.</typeparam>
/// <typeparam name="TResult">Результат.</typeparam>
public interface ILoggingQueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
	where TQuery : IQuery;
