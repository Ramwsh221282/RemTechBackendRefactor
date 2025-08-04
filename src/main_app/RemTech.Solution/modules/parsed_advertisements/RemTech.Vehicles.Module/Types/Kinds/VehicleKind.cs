using RemTech.Vehicles.Module.Types.Kinds.ValueObjects;

namespace RemTech.Vehicles.Module.Types.Kinds;

public class VehicleKind
{
    protected virtual VehicleKindIdentity Identity { get; }

    public VehicleKind(VehicleKindIdentity identity) => Identity = identity;

    public VehicleKind(VehicleKind origin) => Identity = origin.Identity;

    private VehicleKind(VehicleKind origin, VehicleKindIdentity identity)
        : this(origin)
    {
        Identity = identity;
    }

    public string Name() => Identity.ReadText();

    public Guid Id() => Identity.ReadId();

    public VehicleKind ChangeIdentity(VehicleKindIdentity identity)
    {
        return new VehicleKind(this, identity);
    }
}
