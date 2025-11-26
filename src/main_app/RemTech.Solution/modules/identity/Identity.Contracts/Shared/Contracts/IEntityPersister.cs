namespace Identity.Contracts.Shared.Contracts;

public interface IEntityPersister<in TInstance> where TInstance : class
{
    Task Persist(TInstance instance, CancellationToken ct = default);
}