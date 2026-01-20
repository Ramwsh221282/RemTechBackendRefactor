using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;
using Vehicles.Domain.Brands;
using Vehicles.Infrastructure.Brands.Queries.GetBrand;

namespace Vehicles.Infrastructure.Brands.Queries.GetBrands;

public sealed class GetBrandCachingQueryHandler(
    HybridCache cache,
    IQueryHandler<GetBrandQuery, IEnumerable<BrandResponse>> inner
) : IQueryExecutorWithCache<GetBrandQuery, IEnumerable<BrandResponse>>
{
    public async Task<IEnumerable<BrandResponse>> ExecuteWithCache(
        GetBrandQuery query,
        CancellationToken ct = default
    ) => await ReadFromCache(query, CreateCacheKey(query), ct);

    private async Task<IEnumerable<BrandResponse>> ReadFromCache(
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
