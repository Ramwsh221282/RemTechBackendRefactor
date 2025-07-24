using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Adapters.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;

public abstract class VehicleKind(VehicleKindIdentity identity) : IVehicleKind
{
    private readonly VehicleKindIdentity _identity = identity;
    
    public VehicleKindIdentity Identify() => _identity;
    
    public Vehicle Print(Vehicle vehicle) => new(vehicle, this);

    public PgVehicleKindToStoreCommand ToStoreCommand() =>
        new(_identity.ReadText(), _identity.ReadId());

    public PgVehicleKindFromStoreCommand FromStoreCommand() =>
        new(_identity.ReadText());
}
