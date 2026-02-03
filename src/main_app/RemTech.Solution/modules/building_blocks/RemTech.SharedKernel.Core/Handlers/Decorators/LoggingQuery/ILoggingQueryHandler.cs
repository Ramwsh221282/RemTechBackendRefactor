namespace RemTech.SharedKernel.Core.Handlers.Decorators.LoggingQuery;

public interface ILoggingQueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
	where TQuery : IQuery;
