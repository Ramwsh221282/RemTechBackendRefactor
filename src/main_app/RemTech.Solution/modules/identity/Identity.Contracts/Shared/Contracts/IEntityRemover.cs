namespace Identity.Contracts.Shared.Contracts;

public interface IEntityRemover<in TInstance> where TInstance : class
{
    Task Remove(TInstance instance, CancellationToken ct = default);
}