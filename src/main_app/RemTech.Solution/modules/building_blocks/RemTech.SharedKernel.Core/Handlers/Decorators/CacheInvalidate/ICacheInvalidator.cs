namespace RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

public interface ICacheInvalidator<in TCommand, in TResult>
	where TCommand : ICommand
{
	public Task InvalidateCache(TCommand command, TResult result, CancellationToken ct = default);
}
