namespace RemTech.SharedKernel.Core.Handlers;

public interface ILoggingCommandHandler<TCommand, TResult> : ICommandHandler
    <TCommand, TResult> where TCommand : ICommand;