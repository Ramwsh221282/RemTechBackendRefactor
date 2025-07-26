using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;

public class VehicleBrand
{
    protected virtual VehicleBrandIdentity Identity { get; }
    public VehicleBrand(VehicleBrandIdentity identity) => Identity = identity;
    public VehicleBrand(VehicleBrand origin) => Identity = origin.Identity;
    public string Name() => Identity.ReadText();
    public Guid Id() => Identity.ReadId();
}
