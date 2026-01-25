namespace RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

public sealed class CachingQueryHandler<TQuery, TResult>(
	IEnumerable<IQueryExecutorWithCache<TQuery, TResult>> cachedHandlers,
	IQueryHandler<TQuery, TResult> handler
) : IQueryHandler<TQuery, TResult>
	where TQuery : IQuery
{
	private readonly IEnumerable<IQueryExecutorWithCache<TQuery, TResult>> _executors = cachedHandlers;

	public Task<TResult> Handle(TQuery query, CancellationToken ct = default)
	{
		foreach (IQueryExecutorWithCache<TQuery, TResult> executor in _executors)
		{
			return executor.ExecuteWithCache(query, ct);
		}

		return handler.Handle(query, ct);
	}
}
