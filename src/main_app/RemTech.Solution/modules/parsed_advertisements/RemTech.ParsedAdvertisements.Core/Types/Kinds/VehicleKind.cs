using RemTech.ParsedAdvertisements.Core.Types.Kinds.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Types.Kinds;

public class VehicleKind
{
    protected virtual VehicleKindIdentity Identity { get; }

    public VehicleKind(VehicleKindIdentity identity) => Identity = identity;

    public VehicleKind(VehicleKind origin) => Identity = origin.Identity;

    public string Name() => Identity.ReadText();

    public Guid Id() => Identity.ReadId();
}
