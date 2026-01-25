using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

namespace ParsersControl.Infrastructure.Parsers.Queries.GetParsers;

public sealed class GetParsersCachedQueryHandler(
    IQueryHandler<GetParsersQuery, IEnumerable<ParserResponse>> inner,
    HybridCache cache
) : IQueryExecutorWithCache<GetParsersQuery, IEnumerable<ParserResponse>>
{
    public async Task<IEnumerable<ParserResponse>> ExecuteWithCache(
        GetParsersQuery query,
        CancellationToken ct = default
    )
    {
        const string cacheKey = ParserCacheContants.Array;
        return await cache.GetOrCreateAsync(
            cacheKey,
            async cancellationToken =>
            {
                IEnumerable<ParserResponse> response = await inner.Handle(query, cancellationToken);
                return response;
            },
            cancellationToken: ct
        );
    }
}
