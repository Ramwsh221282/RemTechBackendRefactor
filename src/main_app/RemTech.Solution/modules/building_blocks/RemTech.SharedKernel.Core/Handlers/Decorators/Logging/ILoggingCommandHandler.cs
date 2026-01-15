namespace RemTech.SharedKernel.Core.Handlers.Decorators.Logging;

public interface ILoggingCommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand;
