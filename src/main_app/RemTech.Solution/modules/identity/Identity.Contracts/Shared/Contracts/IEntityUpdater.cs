namespace Identity.Contracts.Shared.Contracts;

public interface IEntityUpdater<in TInstance> where TInstance : class
{
    Task Update(TInstance instance, CancellationToken ct = default);
}