using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;

public class VehicleKind
{
    protected virtual VehicleKindIdentity Identity { get; }
    public VehicleKind(VehicleKindIdentity identity) => Identity = identity;
    public VehicleKind(VehicleKind origin) => Identity = origin.Identity;
    public string Name() => Identity.ReadText();
    public Guid Id() => Identity.ReadId();
}
