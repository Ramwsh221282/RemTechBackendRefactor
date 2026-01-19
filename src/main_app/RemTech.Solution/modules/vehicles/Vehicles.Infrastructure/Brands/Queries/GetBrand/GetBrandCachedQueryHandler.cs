using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

namespace Vehicles.Infrastructure.Brands.Queries.GetBrand;

public sealed class GetBrandCachedQueryHandler(
    HybridCache cache,
    ICachingQueryHandler<GetBrandQuery, BrandResponse?> inner
) : IQueryExecutorWithCache<GetBrandQuery, BrandResponse?>
{
    public async Task<BrandResponse?> ExecuteWithCache(
        GetBrandQuery query,
        CancellationToken ct = new()
    ) => await ReadFromCache(query, ct);

    private async Task<BrandResponse?> ReadFromCache(GetBrandQuery query, CancellationToken ct) =>
        await cache.GetOrCreateAsync(
            CreateCacheKey(query),
            async token => await inner.Handle(query, token),
            cancellationToken: ct
        );

    private static string CreateCacheKey(GetBrandQuery query) => $"{nameof(GetBrandQuery)}_{query}";
}
