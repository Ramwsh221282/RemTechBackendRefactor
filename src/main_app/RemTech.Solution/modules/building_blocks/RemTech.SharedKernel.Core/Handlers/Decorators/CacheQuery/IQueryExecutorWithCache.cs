namespace RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

public interface IQueryExecutorWithCache<in TQuery, TResponse>
    where TQuery : IQuery
{
    public Task<TResponse> ExecuteWithCache(TQuery query, CancellationToken ct = default);
}
