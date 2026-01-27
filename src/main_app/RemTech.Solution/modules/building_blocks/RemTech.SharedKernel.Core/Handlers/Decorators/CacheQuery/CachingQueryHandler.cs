namespace RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

/// <summary>
/// Обработчик запросов с кэшированием.
/// </summary>
/// <typeparam name="TQuery">Тип запроса.</typeparam>
/// <typeparam name="TResult">Тип результата.</typeparam>
/// <param name="cachedHandlers">Коллекция обработчиков с кэшем.</param>
/// <param name="handler">Внутренний обработчик запросов.</param>
public sealed class CachingQueryHandler<TQuery, TResult>(
	IEnumerable<IQueryExecutorWithCache<TQuery, TResult>> cachedHandlers,
	IQueryHandler<TQuery, TResult> handler
) : IQueryHandler<TQuery, TResult>
	where TQuery : IQuery
{
	private readonly IEnumerable<IQueryExecutorWithCache<TQuery, TResult>> _executors = cachedHandlers;

	/// <summary>
	/// Обрабатывает запрос с использованием кэша, если доступно.
	/// </summary>
	/// <param name="query">Запрос для обработки.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Результат обработки запроса.</returns>
	public Task<TResult> Handle(TQuery query, CancellationToken ct = default)
	{
		foreach (IQueryExecutorWithCache<TQuery, TResult> executor in _executors)
		{
			return executor.ExecuteWithCache(query, ct);
		}

		return handler.Handle(query, ct);
	}
}
