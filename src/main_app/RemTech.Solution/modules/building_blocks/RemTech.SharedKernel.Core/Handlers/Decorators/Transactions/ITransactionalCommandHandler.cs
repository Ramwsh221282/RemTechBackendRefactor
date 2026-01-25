namespace RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

public interface ITransactionalCommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
	where TCommand : ICommand;
