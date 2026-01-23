using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Brands.Contracts;

public interface IBrandPersister
{
    public Task<Result<Brand>> Save(Brand brand, CancellationToken ct = default);
}
