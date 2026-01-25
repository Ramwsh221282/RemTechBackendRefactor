using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Contracts;

public interface IPersistable<T>
    where T : class
{
    public Task<Result<T>> SaveBy(IPersister persister, CancellationToken ct = default);
}
