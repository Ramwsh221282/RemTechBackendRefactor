namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Ports;

public interface IVehicleBrands
{
    Task<ParsedVehicleBrand> AddOrGet(ParsedVehicleBrand brand, CancellationToken ct = default);
}
