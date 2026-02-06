namespace RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

/// <summary>
/// Интерфейс для обработчиков запросов с кэшированием, используемых в тестах.
/// </summary>
/// <typeparam name="TQuery">Тип запроса.</typeparam>
/// <typeparam name="TResult">Тип результата.</typeparam>
public interface ICachingQueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
	where TQuery : IQuery;
