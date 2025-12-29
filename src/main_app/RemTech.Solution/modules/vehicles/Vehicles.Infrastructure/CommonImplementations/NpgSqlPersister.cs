using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Vehicles.Domain.Brands;
using Vehicles.Domain.Brands.Contracts;
using Vehicles.Domain.Categories;
using Vehicles.Domain.Categories.Contracts;
using Vehicles.Domain.Characteristics;
using Vehicles.Domain.Characteristics.Contracts;
using Vehicles.Domain.Contracts;
using Vehicles.Domain.Locations;
using Vehicles.Domain.Locations.Contracts;
using Vehicles.Domain.Models;
using Vehicles.Domain.Models.Contracts;

namespace Vehicles.Infrastructure.CommonImplementations;

public sealed class NpgSqlPersister(
    IBrandPersister brandPersister,
    IModelsPersister modelPersister,
    ICategoryPersister categoryPersister,
    ICharacteristicsPersister characteristicPersister,
    ILocationsPersister locationsPersister
    ) : IPersister
{
    public Task<Result<Brand>> Save(Brand brand, CancellationToken ct = default) => brandPersister.Save(brand, ct);
    public Task<Result<Model>> Save(Model model, CancellationToken ct = default) => modelPersister.Save(model, ct);
    public Task<Result<Location>> Save(Location location, CancellationToken ct = default) => locationsPersister.Save(location, ct);
    public Task<Result<Category>> Save(Category category, CancellationToken ct = default) => categoryPersister.Save(category, ct);
    public Task<Result<Characteristic>> Save(Characteristic characteristic, CancellationToken ct = default) => characteristicPersister.Save(characteristic, ct);    
}