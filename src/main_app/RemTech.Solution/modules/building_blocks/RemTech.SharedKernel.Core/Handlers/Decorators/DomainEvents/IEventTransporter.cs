namespace RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

public interface IEventTransporter<TCommand, TResult>
    where TCommand : ICommand
{
    Task Transport(TResult result, CancellationToken ct = default);
}
