using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicles;

public sealed class GetVehiclesCachingQueryHandler(
	HybridCache cache,
	IQueryHandler<GetVehiclesQuery, GetVehiclesQueryResponse> inner
) : IQueryExecutorWithCache<GetVehiclesQuery, GetVehiclesQueryResponse>
{
	public async Task<GetVehiclesQueryResponse> ExecuteWithCache(
		GetVehiclesQuery query,
		CancellationToken ct = default
	) => await ReadFromCache(query, FormCacheKey(query), ct);

	private async Task<GetVehiclesQueryResponse> ReadFromCache(
		GetVehiclesQuery query,
		string key,
		CancellationToken ct
	) => await cache.GetOrCreateAsync(key, async token => await inner.Handle(query, token), cancellationToken: ct);

	private static string FormCacheKey(GetVehiclesQuery query) => $"{nameof(GetVehiclesQuery)}_{query.Parameters}";
}
