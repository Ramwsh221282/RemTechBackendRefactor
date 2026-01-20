namespace ContainedItems.Infrastructure.Queries.GetMainPageLastAddedItems;

// public sealed class GetMainPageLastAddedItemsCachingHandler(
//     IQueryHandler<GetMainPageLastAddedItemsQuery, MainPageLastAddedItemsResponse> inner,
//     Microsoft.Extensions.Caching.Hybrid.HybridCache cache
// ) : IQueryExecutorWithCache<GetMainPageLastAddedItemsQuery, MainPageLastAddedItemsResponse>
// {
//     public async Task<MainPageLastAddedItemsResponse> ExecuteWithCache(
//         GetMainPageLastAddedItemsQuery query,
//         CancellationToken ct = default
//     )
//     {
//         string key = nameof(GetMainPageLastAddedItemsQuery);
//         return await cache.GetOrCreateAsync(
//             key,
//             async (cancellationToken) => await inner.Handle(query, cancellationToken),
//             cancellationToken: ct
//         );
//     }
// }
