namespace RemTech.SharedKernel.Core.InfrastructureContracts;

public interface IEntityFetcher<TInstance, in TArgs> 
    where TInstance : class
    where TArgs : EntityFetchArgs
{
    Task<TInstance?> Fetch(TArgs args, CancellationToken ct = default);
}