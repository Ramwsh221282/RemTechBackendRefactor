namespace RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

/// <summary>
/// Интерфейс исполнителя запросов с кэшированием.
/// </summary>
/// <typeparam name="TQuery">Тип запроса.</typeparam>
/// <typeparam name="TResponse">Тип ответа.</typeparam>
public interface IQueryExecutorWithCache<in TQuery, TResponse>
	where TQuery : IQuery
{
	/// <summary>
	/// Выполняет запрос с использованием кэша.
	/// </summary>
	/// <param name="query">Запрос для выполнения.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Результат выполнения запроса.</returns>
	Task<TResponse> ExecuteWithCache(TQuery query, CancellationToken ct = default);
}
