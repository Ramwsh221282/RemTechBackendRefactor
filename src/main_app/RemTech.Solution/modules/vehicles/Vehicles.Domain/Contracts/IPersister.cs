using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Vehicles.Domain.Brands;
using Vehicles.Domain.Categories;
using Vehicles.Domain.Characteristics;
using Vehicles.Domain.Locations;
using Vehicles.Domain.Models;

namespace Vehicles.Domain.Contracts;

public interface IPersister
{
    Task<Result<Brand>> Save(Brand brand, CancellationToken ct = default);
    Task<Result<Model>> Save(Model model, CancellationToken ct = default);
    Task<Result<Location>> Save(Location location, CancellationToken ct = default);
    Task<Result<Category>> Save(Category category, CancellationToken ct = default);
    Task<Result<Characteristic>> Save(Characteristic characteristic, CancellationToken ct = default);
}