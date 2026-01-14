namespace RemTech.SharedKernel.Core.Handlers;

public interface ITransactionalCommandHandler<TCommand, TResult>
    : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand;
