using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;
using Vehicles.Infrastructure.Brands.Queries.GetBrand;

namespace Vehicles.Infrastructure.Brands.Queries.GetBrands;

public sealed class GetBrandCachingQueryHandler(
    HybridCache cache,
    IQueryHandler<GetBrandQuery, BrandResponse?> inner
) : IQueryExecutorWithCache<GetBrandQuery, BrandResponse?>
{
    public async Task<BrandResponse?> ExecuteWithCache(
        GetBrandQuery query,
        CancellationToken ct = default
    ) => await ReadFromCache(query, CreateCacheKey(query), ct);

    private async Task<BrandResponse?> ReadFromCache(
        GetBrandQuery query,
        string key,
        CancellationToken ct
    ) =>
        await cache.GetOrCreateAsync(
            key,
            async token => await inner.Handle(query, token),
            cancellationToken: ct
        );

    private static string CreateCacheKey(GetBrandQuery query) => $"{nameof(GetBrandQuery)}_{query}";
}
