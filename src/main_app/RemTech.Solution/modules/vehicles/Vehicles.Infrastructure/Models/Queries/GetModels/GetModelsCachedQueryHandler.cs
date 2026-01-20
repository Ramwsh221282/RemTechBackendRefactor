using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;
using Vehicles.Infrastructure.Models.Queries.GetModel;

namespace Vehicles.Infrastructure.Models.Queries.GetModels;

public sealed class GetModelsCachedQueryHandler(
    HybridCache cache,
    IQueryHandler<GetModelQuery, IEnumerable<ModelResponse>> inner
) : IQueryExecutorWithCache<GetModelQuery, IEnumerable<ModelResponse>>
{
    public async Task<IEnumerable<ModelResponse>> ExecuteWithCache(
        GetModelQuery query,
        CancellationToken ct = default
    ) => await ReadFromCache(query, CreateCacheKey(query), ct);

    private async Task<IEnumerable<ModelResponse>> ReadFromCache(
        GetModelQuery query,
        string key,
        CancellationToken ct
    ) =>
        await cache.GetOrCreateAsync(
            key,
            async (token) => await inner.Handle(query, token),
            cancellationToken: ct
        );

    private static string CreateCacheKey(GetModelQuery query) => $"{nameof(GetModelQuery)}_{query}";
}
