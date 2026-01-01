namespace RemTech.SharedKernel.Core.Handlers;

public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery
{
    Task<TResponse> Handle(TQuery query, CancellationToken ct = default);
}