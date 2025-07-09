namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Ports;

public interface IVehicleBrands
{
    Task<VehicleBrand> Similar(VehicleBrand brand, CancellationToken ct = default);
}
