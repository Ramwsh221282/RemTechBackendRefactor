namespace RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

/// <summary>
/// Интерфейс транзакционного обработчика команд.
/// </summary>
/// <typeparam name="TCommand">Команда.</typeparam>
/// <typeparam name="TResult">Результат.</typeparam>
public interface ITransactionalCommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
	where TCommand : ICommand;
