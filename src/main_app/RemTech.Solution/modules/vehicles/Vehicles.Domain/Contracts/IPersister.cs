using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Vehicles.Domain.Brands;
using Vehicles.Domain.Categories;
using Vehicles.Domain.Characteristics;
using Vehicles.Domain.Locations;
using Vehicles.Domain.Models;
using Vehicles.Domain.Vehicles.Contracts;

namespace Vehicles.Domain.Contracts;

public interface IPersister
{
    public Task<Result<Brand>> Save(Brand brand, CancellationToken ct = default);
    public Task<Result<Model>> Save(Model model, CancellationToken ct = default);
    public Task<Result<Location>> Save(Location location, CancellationToken ct = default);
    public Task<Result<Category>> Save(Category category, CancellationToken ct = default);
    public Task<Result<Characteristic>> Save(Characteristic characteristic, CancellationToken ct = default);
    public Task<Result<VehiclePersistInfo>> Save(VehiclePersistInfo info, CancellationToken ct = default);
    public Task<int> Save(IEnumerable<VehiclePersistInfo> infos, CancellationToken ct = default);
}
