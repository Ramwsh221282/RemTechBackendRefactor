namespace RemTech.SharedKernel.Core.Handlers;

public interface ICachingQueryHandler<in TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery;
