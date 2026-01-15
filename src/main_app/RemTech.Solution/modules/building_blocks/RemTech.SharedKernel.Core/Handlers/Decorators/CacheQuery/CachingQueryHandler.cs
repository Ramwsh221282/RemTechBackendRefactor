namespace RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

public sealed class CachingQueryHandler<TQuery, TResult>(
    IEnumerable<IQueryExecutorWithCache<TQuery, TResult>> executors
) : ICachingQueryHandler<TQuery, TResult>
    where TQuery : IQuery
{
    private readonly IEnumerable<IQueryExecutorWithCache<TQuery, TResult>> _executors = executors;

    public async Task<TResult> Handle(TQuery query, CancellationToken ct = default)
    {
        foreach (var executor in _executors)
        {
            TResult result = await executor.ExecuteWithCache(query, ct);
            return result;
        }

        throw new Exception(); // This should never happen if at least one executor is registered
    }
}
