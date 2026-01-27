using Microsoft.Extensions.Caching.Hybrid;
using Notifications.Infrastructure.Mailers.Queries.GetMailers;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

namespace Notifications.Infrastructure.Mailers.Queries.GetMailer;

/// <summary>
/// Обработчик запроса на получение mailer с кэшированием.
/// </summary>
/// <param name="cache">Кэш для хранения результатов.</param>
/// <param name="inner">Внутренний обработчик запроса.</param>
public sealed class GetMailerCachedHandler(HybridCache cache, IQueryHandler<GetMailerQuery, MailerResponse?> inner)
	: IQueryExecutorWithCache<GetMailerQuery, MailerResponse?>
{
	/// <summary>
	/// Выполнить запрос с использованием кэша.
	/// </summary>
	/// <param name="query">Запрос на получение mailer.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения запроса.</returns>
	public async Task<MailerResponse?> ExecuteWithCache(GetMailerQuery query, CancellationToken ct = default)
	{
		string key = $"mailer_instance_{query.Id}";

		MailerResponse? cached = await cache.GetOrCreateAsync(
			key,
			async cancellationToken => await inner.Handle(query, cancellationToken),
			cancellationToken: ct
		);

		return cached;
	}
}
