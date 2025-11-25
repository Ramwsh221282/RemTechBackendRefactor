namespace Identity.Contracts.Shared;

public abstract record PersisterQueryArgs;

public interface IPersister<TInstance, in TArgs> 
    where TInstance : class
    where TArgs : PersisterQueryArgs
{
    Task Persist(TInstance instance, CancellationToken ct = default);
    Task Remove(TInstance instance, CancellationToken ct = default);
    Task Update(TInstance instance, CancellationToken ct = default);
    Task<TInstance?> Get(TArgs args, CancellationToken ct = default);
}