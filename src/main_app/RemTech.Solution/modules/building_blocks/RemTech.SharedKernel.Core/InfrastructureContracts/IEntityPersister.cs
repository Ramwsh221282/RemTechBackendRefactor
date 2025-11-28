namespace RemTech.SharedKernel.Core.InfrastructureContracts;

public interface IEntityPersister<in TInstance> where TInstance : class
{
    Task Persist(TInstance instance, CancellationToken ct = default);
}