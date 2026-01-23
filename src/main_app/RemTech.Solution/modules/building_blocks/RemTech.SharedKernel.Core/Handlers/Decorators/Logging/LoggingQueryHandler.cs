using System.Diagnostics;
using Serilog;

namespace RemTech.SharedKernel.Core.Handlers.Decorators.Logging;

public sealed class LoggingQueryHandler<TQuery, TResult>(ILogger logger, IQueryHandler<TQuery, TResult> inner)
	: IQueryHandler<TQuery, TResult>
	where TQuery : IQuery
{
	private ILogger Logger { get; } = logger.ForContext<TQuery>();

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
