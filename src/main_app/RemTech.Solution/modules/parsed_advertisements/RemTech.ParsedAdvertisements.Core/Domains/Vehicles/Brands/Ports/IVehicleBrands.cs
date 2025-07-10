using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Ports;

public interface IVehicleBrands
{
    Status<VehicleBrandEnvelope> Add(string? name);
    MaybeBag<VehicleBrandEnvelope> GetByName(string? name);
}
