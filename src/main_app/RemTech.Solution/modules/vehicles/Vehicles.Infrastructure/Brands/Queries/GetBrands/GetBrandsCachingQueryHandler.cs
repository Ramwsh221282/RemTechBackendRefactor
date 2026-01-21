using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

namespace Vehicles.Infrastructure.Brands.Queries.GetBrands;

public sealed class GetBrandsCachingQueryHandler(
    HybridCache cache,
    IQueryHandler<GetBrandsQuery, IEnumerable<BrandResponse>> inner
) : IQueryExecutorWithCache<GetBrandsQuery, IEnumerable<BrandResponse>>
{
    public async Task<IEnumerable<BrandResponse>> ExecuteWithCache(
        GetBrandsQuery query,
        CancellationToken ct = default
    ) => await ReadFromCache(query, CreateCacheKey(query), ct);

    private async Task<IEnumerable<BrandResponse>> ReadFromCache(
        GetBrandsQuery query,
        string key,
        CancellationToken ct
    ) =>
        await cache.GetOrCreateAsync(
            key,
            async token => await inner.Handle(query, token),
            cancellationToken: ct
        );

    private static string CreateCacheKey(GetBrandsQuery query) =>
        $"{nameof(GetBrandsQuery)}_{query}";
}
