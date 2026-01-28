using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

namespace ParsersControl.Infrastructure.Parsers.Queries.GetParsers;

/// <summary>
///  Обработчик кэшированного запроса на получение всех парсеров.
/// </summary>
/// <param name="inner">Внутренний обработчик запроса получения парсеров.</param>
/// <param name="cache">Экземпляр кэша для хранения результатов запроса.</param>
public sealed class GetParsersCachedQueryHandler(
	IQueryHandler<GetParsersQuery, IEnumerable<ParserResponse>> inner,
	HybridCache cache
) : IQueryExecutorWithCache<GetParsersQuery, IEnumerable<ParserResponse>>
{
	/// <summary>
	/// Выполняет кэшированный запрос на получение всех парсеров.
	/// </summary>
	/// <param name="query">Запрос на получение всех парсеров.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Коллекция ответов с информацией о парсерах.</returns>
	public async Task<IEnumerable<ParserResponse>> ExecuteWithCache(
		GetParsersQuery query,
		CancellationToken ct = default
	)
	{
		const string cacheKey = ParserCacheContants.ARRAY;
		return await cache.GetOrCreateAsync(
			cacheKey,
			async cancellationToken =>
			{
				IEnumerable<ParserResponse> response = await inner.Handle(query, cancellationToken);
				return response;
			},
			cancellationToken: ct
		);
	}
}
