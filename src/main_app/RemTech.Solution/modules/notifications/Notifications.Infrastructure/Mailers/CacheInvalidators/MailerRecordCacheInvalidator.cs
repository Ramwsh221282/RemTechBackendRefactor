using Microsoft.Extensions.Caching.Hybrid;

namespace Notifications.Infrastructure.Mailers.CacheInvalidators;

/// <summary>
/// Инвалидатор кэша для записи почтового ящика.
/// </summary>
/// <param name="cache">Кэш для хранения данных.</param>
public sealed class MailerRecordCacheInvalidator(HybridCache cache)
{
	/// <summary>
	/// Инвалидирует кэш записи почтового ящика.
	/// </summary>
	/// <param name="id">Идентификатор почтового ящика.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инвалидирования кэша.</returns>
	public async Task Invalidate(Guid id, CancellationToken ct = default)
	{
		string key = $"mailer_instance_{id}";
		await cache.RemoveAsync(key, ct);
	}
}
