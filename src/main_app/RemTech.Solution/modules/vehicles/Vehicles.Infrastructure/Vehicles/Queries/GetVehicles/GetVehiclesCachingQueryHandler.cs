using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicles;

/// <summary>
/// Обработчик кэширования для запроса получения транспортных средств.
/// </summary>
/// <param name="cache">Кэш для хранения результатов запроса.</param>
/// <param name="inner">Внутренний обработчик запроса.</param>
public sealed class GetVehiclesCachingQueryHandler(
	HybridCache cache,
	IQueryHandler<GetVehiclesQuery, GetVehiclesQueryResponse> inner
) : IQueryExecutorWithCache<GetVehiclesQuery, GetVehiclesQueryResponse>
{
	/// <summary>
	/// Выполняет запрос с использованием кэша.
	/// </summary>
	/// <param name="query">Запрос на получение транспортных средств.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Ответ с транспортными средствами.</returns>
	public Task<GetVehiclesQueryResponse> ExecuteWithCache(GetVehiclesQuery query, CancellationToken ct = default)
	{
		return ReadFromCache(query, FormCacheKey(query), ct);
	}

	private static string FormCacheKey(GetVehiclesQuery query)
	{
		return $"{nameof(GetVehiclesQuery)}_{query.Parameters}";
	}

	private async Task<GetVehiclesQueryResponse> ReadFromCache(GetVehiclesQuery query, string key, CancellationToken ct)
	{
		return await cache.GetOrCreateAsync(
			key,
			async token => await inner.Handle(query, token),
			cancellationToken: ct
		);
	}
}
