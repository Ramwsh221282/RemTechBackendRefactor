namespace RemTech.SharedKernel.Core.Handlers;

public interface IValidatingCommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand;