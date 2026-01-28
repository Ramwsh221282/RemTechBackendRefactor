using System.Diagnostics;
using Serilog;

namespace RemTech.SharedKernel.Core.Handlers.Decorators.Logging;

/// <summary>
/// Декоратор логирующего обработчика запросов.
/// </summary>
/// <typeparam name="TQuery">Запрос.</typeparam>
/// <typeparam name="TResult">Результат.</typeparam>
/// <param name="logger">Логгер.</param>
/// <param name="inner">Внутренний обработчик запроса.</param>
public sealed class LoggingQueryHandler<TQuery, TResult>(ILogger logger, IQueryHandler<TQuery, TResult> inner)
	: IQueryHandler<TQuery, TResult>
	where TQuery : IQuery
{
	private ILogger Logger { get; } = logger.ForContext<TQuery>();

	/// <summary>
	/// Обрабатывает запрос с логированием.
	/// </summary>
	/// <param name="query">Запрос для обработки.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Результат обработки запроса.</returns>
	public async Task<TResult> Handle(TQuery query, CancellationToken ct = default)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		Logger.Information(
			"""
			Executing query
			Query payload: {Payload}
			""",
			typeof(TQuery).Name,
			query.ToString()
		);
		TResult result = await inner.Handle(query, ct);
		Logger.Information(
			"""
			Query executed in {ElapsedMilliseconds} ms.
			""",
			stopwatch.ElapsedMilliseconds
		);
		return result;
	}
}
