namespace RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

public interface ICacheInvalidatingHandler<in TCommand, TResult> : ICommandHandler<TCommand, TResult>
	where TCommand : ICommand { }
