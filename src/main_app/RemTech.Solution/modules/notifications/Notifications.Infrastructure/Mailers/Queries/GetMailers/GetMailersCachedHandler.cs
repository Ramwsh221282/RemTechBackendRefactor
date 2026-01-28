using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

namespace Notifications.Infrastructure.Mailers.Queries.GetMailers;

/// <summary>
/// Кешированный обработчик запроса получения множества почтовых ящиков.
/// </summary>
/// <param name="cache">Кеш для хранения результатов запроса.</param>
/// <param name="inner">Внутренний обработчик запроса.</param>
public sealed class GetMailersCachedHandler(
	HybridCache cache,
	IQueryHandler<GetMailersQuery, IEnumerable<MailerResponse>> inner
) : IQueryExecutorWithCache<GetMailersQuery, IEnumerable<MailerResponse>>
{
	/// <summary>
	///   Выполняет запрос получения множества почтовых ящиков с использованием кеша.
	/// </summary>
	/// <param name="query">Запрос на получение множества почтовых ящиков.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Коллекция ответов с данными почтовых ящиков.</returns>
	public async Task<IEnumerable<MailerResponse>> ExecuteWithCache(
		GetMailersQuery query,
		CancellationToken ct = default
	)
	{
		const string cacheKey = "mailers_array";
		return await cache.GetOrCreateAsync(
			cacheKey,
			async cancellationToken => await inner.Handle(query, cancellationToken),
			cancellationToken: ct
		);
	}
}
