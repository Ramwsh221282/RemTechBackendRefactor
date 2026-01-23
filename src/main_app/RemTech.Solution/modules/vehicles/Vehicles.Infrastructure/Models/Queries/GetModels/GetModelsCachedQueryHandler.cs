using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

namespace Vehicles.Infrastructure.Models.Queries.GetModels;

public sealed class GetModelsCachedQueryHandler(
	HybridCache cache,
	IQueryHandler<GetModelsQuery, IEnumerable<ModelResponse>> inner
) : IQueryExecutorWithCache<GetModelsQuery, IEnumerable<ModelResponse>>
{
	public async Task<IEnumerable<ModelResponse>> ExecuteWithCache(
		GetModelsQuery query,
		CancellationToken ct = default
	) => await ReadFromCache(query, CreateCacheKey(query), ct);

	private async Task<IEnumerable<ModelResponse>> ReadFromCache(
		GetModelsQuery query,
		string key,
		CancellationToken ct
	) => await cache.GetOrCreateAsync(key, async (token) => await inner.Handle(query, token), cancellationToken: ct);

	private static string CreateCacheKey(GetModelsQuery query) => $"{nameof(GetModelsQuery)}_{query}";
}
