using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Brands;

public interface IAsyncVehicleBrands : IDisposable, IAsyncDisposable
{
    Task<Status<IVehicleBrand>> Add(IVehicleBrand brand, CancellationToken ct = default);
    Task<MaybeBag<IVehicleBrand>> Find(
        VehicleBrandIdentity identity,
        CancellationToken ct = default
    );
}
