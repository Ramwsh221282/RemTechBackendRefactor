using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

namespace ContainedItems.Infrastructure.Queries.GetMainPageItemStats;

// public sealed class GetMainPageItemStatsCachingHandler(
//     IQueryHandler<GetMainPageItemStatsQuery, MainPageItemStatsResponse> inner,
//     HybridCache cache
// ) : IQueryExecutorWithCache<GetMainPageItemStatsQuery, MainPageItemStatsResponse>
// {
//     public async Task<MainPageItemStatsResponse> ExecuteWithCache(
//         GetMainPageItemStatsQuery query,
//         CancellationToken ct = default
//     )
//     {
//         string key = nameof(GetMainPageItemStatsQuery);
//         return await cache.GetOrCreateAsync(
//             key,
//             async (cancellationToken) => await inner.Handle(query, cancellationToken),
//             cancellationToken: ct
//         );
//     }
// }
