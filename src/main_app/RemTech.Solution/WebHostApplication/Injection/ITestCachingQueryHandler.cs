using RemTech.SharedKernel.Core.Handlers;

namespace WebHostApplication.Injection;

/// <summary>
/// Интерфейс для обработчиков запросов с кэшированием, используемых в тестах.
/// </summary>
/// <typeparam name="TQuery">Тип запроса.</typeparam>
/// <typeparam name="TResult">Тип результата.</typeparam>
public interface ITestCachingQueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
	where TQuery : IQuery;
