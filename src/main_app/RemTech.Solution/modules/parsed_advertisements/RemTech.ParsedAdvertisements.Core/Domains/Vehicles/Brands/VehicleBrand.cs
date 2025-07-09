using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;

public interface IVehicleBrand
{
    VehicleBrandIdentity Identify();
}

public sealed class VehicleBrand : IVehicleBrand
{
    private readonly VehicleBrandIdentity _identity;

    public VehicleBrand(VehicleBrandText text)
        : this(new VehicleBrandIdentity(text)) { }

    public VehicleBrand(VehicleBrandId id, VehicleBrandText text)
        : this(new VehicleBrandIdentity(id, text)) { }

    public VehicleBrand(VehicleBrandIdentity identity)
    {
        _identity = identity;
    }

    public VehicleBrandIdentity Identify() => _identity;
}
