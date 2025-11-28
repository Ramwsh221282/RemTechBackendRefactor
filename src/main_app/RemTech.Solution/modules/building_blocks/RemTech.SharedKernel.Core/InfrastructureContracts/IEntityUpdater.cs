namespace RemTech.SharedKernel.Core.InfrastructureContracts;

public interface IEntityUpdater<in TInstance> where TInstance : class
{
    Task Update(TInstance instance, CancellationToken ct = default);
}