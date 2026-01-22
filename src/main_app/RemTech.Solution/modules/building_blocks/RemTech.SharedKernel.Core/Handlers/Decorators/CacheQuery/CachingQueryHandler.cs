namespace RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

public sealed class CachingQueryHandler<TQuery, TResult>(
    IEnumerable<IQueryExecutorWithCache<TQuery, TResult>> cachedHandlers,
    IQueryHandler<TQuery, TResult> handler
) : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery
{
    private readonly IEnumerable<IQueryExecutorWithCache<TQuery, TResult>> _executors =
        cachedHandlers;

    public async Task<TResult> Handle(TQuery query, CancellationToken ct = default)
    {
        foreach (var executor in _executors)
        {
            TResult result = await executor.ExecuteWithCache(query, ct);
            return result;
        }

        return await handler.Handle(query, ct);
    }
}
