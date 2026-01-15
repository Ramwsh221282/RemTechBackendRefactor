namespace RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

public interface ICachingQueryHandler<in TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery;
