namespace RemTech.Core.Shared.Cqrs;

public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand
{
    Task<TResult> Handle(TCommand command, CancellationToken ct = default);
}
