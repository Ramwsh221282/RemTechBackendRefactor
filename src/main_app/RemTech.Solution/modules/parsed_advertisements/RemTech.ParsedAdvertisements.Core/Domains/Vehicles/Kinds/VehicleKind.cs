using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;

public sealed class VehicleKind : IVehicleKind
{
    private readonly VehicleKindIdentity _identity;

    public VehicleKind(VehicleKindText text)
        : this(new VehicleKindIdentity(text)) { }

    public VehicleKind(VehicleKindId id, VehicleKindText text)
        : this(new VehicleKindIdentity(id, text)) { }

    public VehicleKind(VehicleKindIdentity identity)
    {
        _identity = identity;
    }

    public VehicleKindIdentity Identify() => _identity;
}
