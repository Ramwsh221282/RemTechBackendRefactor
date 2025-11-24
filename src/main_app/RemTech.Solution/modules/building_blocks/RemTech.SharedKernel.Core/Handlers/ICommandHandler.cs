namespace RemTech.SharedKernel.Core.Handlers;

public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand
{
    Task<TResult> Execute(TCommand command);
}