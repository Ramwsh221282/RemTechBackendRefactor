namespace RemTech.SharedKernel.Core.InfrastructureContracts;

public interface IEntityRemover<in TInstance> where TInstance : class
{
    Task Remove(TInstance instance, CancellationToken ct = default);
}