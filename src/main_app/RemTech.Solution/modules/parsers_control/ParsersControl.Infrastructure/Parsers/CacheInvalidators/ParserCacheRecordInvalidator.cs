using Microsoft.Extensions.Caching.Hybrid;
using ParsersControl.Core.Parsers.Models;

namespace ParsersControl.Infrastructure.Parsers.CacheInvalidators;

/// <summary>
/// Инвалидатор кэша для записи парсера.
/// </summary>
/// <param name="cache">Экземпляр кэша для управления кэшированными данными.</param>
public sealed class ParserCacheRecordInvalidator(HybridCache cache)
{
	/// <summary>
	/// Инвалидирует кэш для записи парсера.
	/// </summary>
	/// <param name="id">Идентификатор записи парсера.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инвалидирования кэша.</returns>
	public async Task Invalidate(Guid id, CancellationToken ct = default)
	{
		string cacheKey = $"get_parser_{id}";
		await cache.RemoveAsync(cacheKey, cancellationToken: ct);
	}

	/// <summary>
	/// Инвалидирует кэш для записи парсера по идентификатору подписанного парсера.
	/// </summary>
	/// <param name="id">Идентификатор подписанного парсера.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инвалидирования кэша.</returns>
	public Task Invalidate(SubscribedParserId id, CancellationToken ct = default)
	{
		return Invalidate(id.Value, ct);
	}

	/// <summary>
	/// Инвалидирует кэш для записи парсера по подписанному парсеру.
	/// </summary>
	/// <param name="parser">Подписанный парсер.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инвалидирования кэша.</returns>
	public Task Invalidate(SubscribedParser parser, CancellationToken ct = default)
	{
		return Invalidate(parser.Id.Value, ct);
	}
}
