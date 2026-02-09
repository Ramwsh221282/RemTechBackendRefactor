using System.Diagnostics;

namespace RemTech.SharedKernel.Core.Handlers.Decorators.LoggingQuery;

public sealed class LoggingQueryHandler<TQuery, TResult> : ILoggingQueryHandler<TQuery, TResult>
	where TQuery : IQuery
{
	private readonly IQueryHandler<TQuery, TResult> _inner;
	private readonly Serilog.ILogger _logger;

	public LoggingQueryHandler(IQueryHandler<TQuery, TResult> inner, Serilog.ILogger logger)
	{
		_inner = inner;
		_logger = logger.ForContext<TQuery>();
	}

	public async Task<TResult> Handle(TQuery query, CancellationToken ct = default)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		LogEntry();
		TResult result = await _inner.Handle(query, ct);
		LogFinish(stopwatch);
		return result;
	}

	private void LogEntry()
	{
		string name = typeof(TQuery).Name;
		_logger.Information(
			"""
			Query: {Name}
			Query payload: {Payload}
			""",
			typeof(TQuery).Name,
			name
		);
	}

	private void LogFinish(Stopwatch stopwatch)
	{
		string name = typeof(TQuery).Name;
		_logger.Information(
			"""
			Query {Name}. 
			Executed in {ElapsedMilliseconds} ms.
			""",
			stopwatch.ElapsedMilliseconds,
			name
		);
	}
}
