namespace RemTech.SharedKernel.Core.Handlers;

public sealed class CachedQueryExecutor<TQuery, TResponse>(
    IEnumerable<ICachingQueryHandler<TQuery, TResponse>> handlers
) : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery
{
    public async Task<TResponse> Handle(TQuery query, CancellationToken ct = default)
    {
        foreach (ICachingQueryHandler<TQuery, TResponse> handler in handlers)
        {
            TResponse response = await handler.Handle(query, ct);
            return response;
        }

        // the IEnumerable<ICachingQueryHandler<TQuery, TResponse>> already contains at least one handler.
        throw new Exception();
    }
}
