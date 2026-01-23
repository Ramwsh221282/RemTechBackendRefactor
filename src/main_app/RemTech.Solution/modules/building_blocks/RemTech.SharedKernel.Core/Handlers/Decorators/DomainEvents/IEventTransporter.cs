namespace RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

public interface IEventTransporter<in TCommand, in TResult>
	where TCommand : ICommand
{
	public Task Transport(TResult result, CancellationToken ct = default);
}
