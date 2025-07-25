using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Adapters.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;

public class VehicleKind : IVehicleKind
{
    protected virtual VehicleKindIdentity Identity { get; }

    public VehicleKind(VehicleKindIdentity identity)
    {
        Identity = identity;
    }

    public VehicleKind(VehicleKind origin)
    {
        Identity = origin.Identity;
    }
    
    public VehicleKindIdentity Identify() => Identity;
    
    public Vehicle Print(Vehicle vehicle) => new Vehicle(vehicle, this);

    public PgVehicleKindToStoreCommand ToStoreCommand() =>
        new(Identity.ReadText(), Identity.ReadId());

    public PgVehicleKindFromStoreCommand FromStoreCommand() =>
        new(Identity.ReadText());
}
