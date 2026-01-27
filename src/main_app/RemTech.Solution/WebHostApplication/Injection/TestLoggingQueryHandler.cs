using System.Diagnostics;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Logging;

namespace WebHostApplication.Injection;

// TODO move to shared kernel core.

/// <summary>
/// Обработчик запросов с логированием для использования в тестах.
/// </summary>
/// <typeparam name="TQuery">Тип запроса.</typeparam>
/// <typeparam name="TResult">Тип результата.</typeparam>
/// <param name="logger">Логгер для записи информации.</param>
/// <param name="inner">Внутренний обработчик запросов.</param>
public sealed class TestLoggingQueryHandler<TQuery, TResult>(
	Serilog.ILogger logger,
	IQueryHandler<TQuery, TResult> inner
) : ILoggingQueryHandler<TQuery, TResult>
	where TQuery : IQuery
{
	private IQueryHandler<TQuery, TResult> Inner { get; } = inner;
	private Serilog.ILogger Logger { get; } = logger.ForContext<TQuery>();

	/// <summary>
	/// Обрабатывает запрос с логированием.
	/// </summary>
	/// <param name="query">Запрос для обработки.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Результат обработки запроса.</returns>
	public async Task<TResult> Handle(TQuery query, CancellationToken ct = default)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		LogEntry(query);
		TResult result = await Inner.Handle(query, ct);
		LogFinish(stopwatch);
		return result;
	}

	private void LogEntry(TQuery query) =>
		Logger.Information(
			"""
			Executing query: {Query}
			Query payload: {Payload}
			""",
			typeof(TQuery).Name,
			query.ToString()
		);

	private void LogFinish(Stopwatch stopwatch) =>
		Logger.Information(
			"""
			Query executed in {ElapsedMilliseconds} ms.
			""",
			stopwatch.ElapsedMilliseconds
		);
}
