using RemTech.Vehicles.Module.Types.Brands.ValueObjects;

namespace RemTech.Vehicles.Module.Types.Brands;

public class VehicleBrand
{
    protected virtual VehicleBrandIdentity Identity { get; }

    public VehicleBrand(VehicleBrandIdentity identity) => Identity = identity;

    public VehicleBrand(VehicleBrand origin) => Identity = origin.Identity;

    private VehicleBrand(VehicleBrand origin, VehicleBrandIdentity identity)
        : this(origin)
    {
        Identity = identity;
    }

    public string Name() => Identity.ReadText();

    public Guid Id() => Identity.ReadId();

    public VehicleBrand ChangeIdentity(VehicleBrandIdentity identity)
    {
        return new VehicleBrand(this, identity);
    }
}
