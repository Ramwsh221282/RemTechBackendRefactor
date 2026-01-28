using Microsoft.Extensions.Caching.Hybrid;

namespace Notifications.Infrastructure.Mailers.CacheInvalidators;

/// <summary>
/// Инвалидатор кэша для массива почтовых ящиков.
/// </summary>
/// <param name="cache">Кэш для хранения данных.</param>
public sealed class MailerArrayCacheInvalidator(HybridCache cache)
{
	/// <summary>
	/// Инвалидирует кэш массива почтовых ящиков.
	/// </summary>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инвалидирования кэша.</returns>
	public async Task Invalidate(CancellationToken ct = default)
	{
		const string key = "mailers_array";
		await cache.RemoveAsync(key, ct);
	}
}
