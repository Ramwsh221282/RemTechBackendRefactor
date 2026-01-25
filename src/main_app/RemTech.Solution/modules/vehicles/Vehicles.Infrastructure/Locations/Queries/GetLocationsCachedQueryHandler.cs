using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

namespace Vehicles.Infrastructure.Locations.Queries;

public sealed class GetLocationsCachedQueryHandler(
	HybridCache cache,
	IQueryHandler<GetLocationsQuery, IEnumerable<LocationsResponse>> handler
) : IQueryExecutorWithCache<GetLocationsQuery, IEnumerable<LocationsResponse>>
{
	public Task<IEnumerable<LocationsResponse>> ExecuteWithCache(
		GetLocationsQuery query,
		CancellationToken ct = default
	) => ReadFromCache(query, CreateCacheKey(query), ct);

	private static string CreateCacheKey(GetLocationsQuery query) => $"{nameof(GetLocationsQuery)}_{query}";

	private async Task<IEnumerable<LocationsResponse>> ReadFromCache(
		GetLocationsQuery query,
		string key,
		CancellationToken ct
	) => await cache.GetOrCreateAsync(key, async (token) => await handler.Handle(query, token), cancellationToken: ct);
}
