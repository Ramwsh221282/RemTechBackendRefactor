using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators.Logic;

public sealed class KindingVehicleKind(VehicleKind kind) : VehicleKind(kind)
{
    public Vehicle KindVehicle(Vehicle vehicle) => new(vehicle, this);
}