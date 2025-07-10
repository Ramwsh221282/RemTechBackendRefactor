using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;

public abstract class VehicleBrandEnvelope(VehicleBrandIdentity identity) : IVehicleBrand
{
    private readonly VehicleBrandIdentity _identity = identity;

    public VehicleBrandIdentity Identify() => _identity;
}
