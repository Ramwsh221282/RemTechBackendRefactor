using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;

public interface IVehicleBrand
{
    VehicleBrandIdentity Identify();
}