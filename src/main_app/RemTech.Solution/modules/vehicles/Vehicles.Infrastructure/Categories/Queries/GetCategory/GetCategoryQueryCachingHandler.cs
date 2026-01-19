using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

namespace Vehicles.Infrastructure.Categories.Queries.GetCategory;

public sealed class GetCategoryQueryCachingHandler
    : IQueryExecutorWithCache<GetCategoryQuery, CategoryResponse?>
{
    private HybridCache Cache { get; }
    private IQueryHandler<GetCategoryQuery, CategoryResponse?> Inner { get; }

    public GetCategoryQueryCachingHandler(
        HybridCache cache,
        IQueryHandler<GetCategoryQuery, CategoryResponse?> inner
    )
    {
        Cache = cache;
        Inner = inner;
    }

    public async Task<CategoryResponse?> ExecuteWithCache(
        GetCategoryQuery query,
        CancellationToken ct = default
    ) => await ReadFromCache(query, CreateCacheKey(query), ct);

    private async Task<CategoryResponse?> ReadFromCache(
        GetCategoryQuery query,
        string key,
        CancellationToken ct
    ) =>
        await Cache.GetOrCreateAsync(
            key,
            async cancellationToken => await Inner.Handle(query, cancellationToken),
            cancellationToken: ct
        );

    private static string CreateCacheKey(GetCategoryQuery query) =>
        $"{nameof(GetCategoryQuery)}_{query}";
}
