using Microsoft.Extensions.Caching.Hybrid;
using ParsersControl.Infrastructure.Parsers.Queries.GetParsers;

namespace ParsersControl.Infrastructure.Parsers.CacheInvalidators;

/// <summary>
/// Инвалидатор кэша для массива парсеров.
/// </summary>
/// <param name="cache">Экземпляр кэша для управления кэшированными данными.</param>
public sealed class CachedParserArrayInvalidator(HybridCache cache)
{
	/// <summary>
	/// Инвалидирует кэш для массива парсеров.
	/// </summary>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инвалидирования кэша.</returns>
	public async Task Invalidate(CancellationToken ct = default) =>
		await cache.RemoveAsync(ParserCacheContants.ARRAY, cancellationToken: ct);
}
