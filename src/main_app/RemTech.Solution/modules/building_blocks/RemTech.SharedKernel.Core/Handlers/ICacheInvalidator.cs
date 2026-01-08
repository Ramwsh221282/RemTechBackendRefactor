namespace RemTech.SharedKernel.Core.Handlers;

public interface ICacheInvalidator<TCommand, TResult> where TCommand : ICommand
{
    Task InvalidateCache(TCommand command, TResult result, CancellationToken ct = default);
}