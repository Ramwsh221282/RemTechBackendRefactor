namespace RemTech.SharedKernel.Core.Handlers;

public interface ICacheInvalidatingHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand
{
    
}