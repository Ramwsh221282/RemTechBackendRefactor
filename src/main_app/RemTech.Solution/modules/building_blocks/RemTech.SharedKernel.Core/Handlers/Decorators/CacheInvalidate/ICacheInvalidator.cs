namespace RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

public interface ICacheInvalidator<TCommand, TResult>
	where TCommand : ICommand
{
	Task InvalidateCache(TCommand command, TResult result, CancellationToken ct = default);
}
