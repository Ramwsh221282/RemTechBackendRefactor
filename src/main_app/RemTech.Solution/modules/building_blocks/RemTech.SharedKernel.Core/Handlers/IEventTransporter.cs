namespace RemTech.SharedKernel.Core.Handlers;

public interface IEventTransporter<TCommand, TResult> where TCommand : ICommand
{
    Task Transport(TResult result, CancellationToken ct = default);
}