using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

namespace Vehicles.Infrastructure.Brands.Queries.GetBrands;

/// <summary>
/// Обработчик запроса получения брендов с кэшированием.
/// </summary>
/// <param name="cache">Кэш для хранения результатов запроса.</param>
/// <param name="inner">Внутренний обработчик запроса.</param>
public sealed class GetBrandsCachingQueryHandler(
	HybridCache cache,
	IQueryHandler<GetBrandsQuery, IEnumerable<BrandResponse>> inner
) : IQueryExecutorWithCache<GetBrandsQuery, IEnumerable<BrandResponse>>
{
	/// <summary>
	/// Выполняет запрос получения брендов с использованием кэша.
	/// </summary>
	/// <param name="query">Запрос получения брендов.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Коллекция ответов с информацией о брендах.</returns>
	public Task<IEnumerable<BrandResponse>> ExecuteWithCache(GetBrandsQuery query, CancellationToken ct = default)
	{
		return ReadFromCache(query, CreateCacheKey(query), ct);
	}

	private static string CreateCacheKey(GetBrandsQuery query)
	{
		return $"{nameof(GetBrandsQuery)}_{query}";
	}

	private async Task<IEnumerable<BrandResponse>> ReadFromCache(GetBrandsQuery query, string key, CancellationToken ct)
	{
		return await cache.GetOrCreateAsync(
			key,
			async token => await inner.Handle(query, token),
			cancellationToken: ct
		);
	}
}
