using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;

public abstract class VehicleKindEnvelope(VehicleKindIdentity identity) : IVehicleKind
{
    private readonly VehicleKindIdentity _identity = identity;

    public VehicleKindIdentity Identify() => _identity;
}
