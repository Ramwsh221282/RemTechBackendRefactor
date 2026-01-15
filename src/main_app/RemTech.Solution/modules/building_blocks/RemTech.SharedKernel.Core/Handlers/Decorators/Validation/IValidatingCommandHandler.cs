namespace RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

public interface IValidatingCommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand;
