namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Ports.Storage;

public interface IPgVehicleBrandsStorage
{
    Task<VehicleBrand> Get(VehicleBrand brand, CancellationToken ct = default);
}