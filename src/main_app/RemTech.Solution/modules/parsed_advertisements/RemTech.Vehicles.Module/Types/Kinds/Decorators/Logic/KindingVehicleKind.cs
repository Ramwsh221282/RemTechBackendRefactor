using RemTech.Vehicles.Module.Types.Transport;

namespace RemTech.Vehicles.Module.Types.Kinds.Decorators.Logic;

public sealed class KindingVehicleKind(VehicleKind kind) : VehicleKind(kind)
{
    public Vehicle KindVehicle(Vehicle vehicle) => new(vehicle, this);
}
