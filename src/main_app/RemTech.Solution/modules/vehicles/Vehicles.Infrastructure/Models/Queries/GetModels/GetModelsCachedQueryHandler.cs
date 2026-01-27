using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

namespace Vehicles.Infrastructure.Models.Queries.GetModels;

/// <summary>
/// Обработчик запроса получения моделей с кэшированием.
/// </summary>
/// <param name="cache">Кэш для хранения результатов запроса.</param>
/// <param name="inner">Внутренний обработчик запроса.</param>
public sealed class GetModelsCachedQueryHandler(
	HybridCache cache,
	IQueryHandler<GetModelsQuery, IEnumerable<ModelResponse>> inner
) : IQueryExecutorWithCache<GetModelsQuery, IEnumerable<ModelResponse>>
{
	/// <summary>
	/// Выполняет запрос получения моделей с использованием кэша.
	/// </summary>
	/// <param name="query">Запрос на получение моделей.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения запроса - коллекция моделей.</returns>
	public Task<IEnumerable<ModelResponse>> ExecuteWithCache(GetModelsQuery query, CancellationToken ct = default) =>
		ReadFromCache(query, CreateCacheKey(query), ct);

	private static string CreateCacheKey(GetModelsQuery query) => $"{nameof(GetModelsQuery)}_{query}";

	private async Task<IEnumerable<ModelResponse>> ReadFromCache(
		GetModelsQuery query,
		string key,
		CancellationToken ct
	) => await cache.GetOrCreateAsync(key, async (token) => await inner.Handle(query, token), cancellationToken: ct);
}
