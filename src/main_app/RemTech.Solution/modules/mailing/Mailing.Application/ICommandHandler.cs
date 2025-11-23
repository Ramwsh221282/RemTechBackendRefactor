namespace Mailing.Application;

public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand
{
    Task<TResult> Execute(TCommand args);
}