using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Brands.Contracts;

public interface IBrandPersister
{
    Task<Result<Brand>> Save(Brand brand, CancellationToken ct = default);
}