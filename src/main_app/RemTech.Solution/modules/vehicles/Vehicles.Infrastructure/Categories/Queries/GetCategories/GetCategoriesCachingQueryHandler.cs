using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

namespace Vehicles.Infrastructure.Categories.Queries.GetCategories;

/// <summary>
/// Обработчик запроса получения категорий с кэшированием.
/// </summary>
/// <param name="cache">Кэш для хранения результатов запросов.</param>
/// <param name="inner">Внутренний обработчик запроса без кэширования.</param>
public sealed class GetCategoriesCachingQueryHandler(
	HybridCache cache,
	IQueryHandler<GetCategoriesQuery, IEnumerable<CategoryResponse>> inner
) : IQueryExecutorWithCache<GetCategoriesQuery, IEnumerable<CategoryResponse>>
{
	/// <summary>
	/// Выполняет запрос получения категорий с использованием кэша.
	/// </summary>
	/// <param name="query">Запрос на получение категорий.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Коллекция ответов с информацией о категориях.</returns>
	public Task<IEnumerable<CategoryResponse>> ExecuteWithCache(
		GetCategoriesQuery query,
		CancellationToken ct = default
	)
	{
		return ReadFromCache(query, CreateCacheKey(query), ct);
	}

	private static string CreateCacheKey(GetCategoriesQuery query)
	{
		return $"{nameof(GetCategoriesQuery)}_{query}";
	}

	private async Task<IEnumerable<CategoryResponse>> ReadFromCache(
		GetCategoriesQuery query,
		string key,
		CancellationToken ct
	)
	{
		return await cache.GetOrCreateAsync(
			key,
			async cancellationToken => await inner.Handle(query, cancellationToken),
			cancellationToken: ct
		);
	}
}
