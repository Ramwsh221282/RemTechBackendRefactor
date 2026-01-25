using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

namespace Vehicles.Infrastructure.Categories.Queries.GetCategories;

public sealed class GetCategoriesCachingQueryHandler(
	HybridCache cache,
	IQueryHandler<GetCategoriesQuery, IEnumerable<CategoryResponse>> inner
) : IQueryExecutorWithCache<GetCategoriesQuery, IEnumerable<CategoryResponse>>
{
	public Task<IEnumerable<CategoryResponse>> ExecuteWithCache(
		GetCategoriesQuery query,
		CancellationToken ct = default
	) => ReadFromCache(query, CreateCacheKey(query), ct);

	private static string CreateCacheKey(GetCategoriesQuery query) => $"{nameof(GetCategoriesQuery)}_{query}";

	private async Task<IEnumerable<CategoryResponse>> ReadFromCache(
		GetCategoriesQuery query,
		string key,
		CancellationToken ct
	) =>
		await cache.GetOrCreateAsync(
			key,
			async cancellationToken => await inner.Handle(query, cancellationToken),
			cancellationToken: ct
		);
}
