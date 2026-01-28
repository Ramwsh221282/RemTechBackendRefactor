using Microsoft.Extensions.Caching.Hybrid;
using ParsersControl.Infrastructure.Parsers.Queries.GetParsers;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

namespace ParsersControl.Infrastructure.Parsers.Queries.GetParser;

/// <summary>
///     Обработчик запроса получения парсера с кэшированием.
/// </summary>
/// <param name="inner">Внутренний обработчик запроса получения парсера.</param>
/// <param name="cache">Экземпляр кэша для хранения результатов запроса.</param>
public sealed class GetParserCachedQueryHandler(IQueryHandler<GetParserQuery, ParserResponse?> inner, HybridCache cache)
	: IQueryExecutorWithCache<GetParserQuery, ParserResponse?>
{
	private IQueryHandler<GetParserQuery, ParserResponse?> Inner { get; } = inner;
	private HybridCache Cache { get; } = cache;

	/// <summary>
	///     Выполняет запрос получения парсера с использованием кэша.
	/// </summary>
	/// <param name="query">Запрос получения парсера.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения запроса - парсер или null, если парсер не найден.</returns>
	public async Task<ParserResponse?> ExecuteWithCache(GetParserQuery query, CancellationToken ct = default)
	{
		string cacheKey = $"get_parser_{query.Id}";
		return await Cache.GetOrCreateAsync(
			key: cacheKey,
			factory: async cancellationToken => await Inner.Handle(query, cancellationToken),
			cancellationToken: ct
		);
	}
}
