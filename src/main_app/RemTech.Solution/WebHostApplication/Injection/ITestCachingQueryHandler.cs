using RemTech.SharedKernel.Core.Handlers;

namespace WebHostApplication.Injection;

public interface ITestCachingQueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
	where TQuery : IQuery;
