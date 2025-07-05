namespace RemTech.Cqrs.Library;

public interface IReading<in TQuery, TResult>
    where TQuery : IRead<TResult>
{
    Task<TResult> Read(TQuery query, CancellationToken ct);
}
