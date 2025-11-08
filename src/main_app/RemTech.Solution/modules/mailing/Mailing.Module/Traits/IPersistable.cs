namespace Mailing.Module.Traits;

public interface IPersistable<T> where T : IPersistenceEngine
{
    Task Persist(T engine, CancellationToken ct = default);
    Task Remove(T engine, CancellationToken ct = default);
    Task Update(T engine, CancellationToken ct = default);
}