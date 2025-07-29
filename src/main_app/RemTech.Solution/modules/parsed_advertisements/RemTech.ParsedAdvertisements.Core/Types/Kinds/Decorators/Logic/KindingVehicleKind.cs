using RemTech.ParsedAdvertisements.Core.Types.Transport;

namespace RemTech.ParsedAdvertisements.Core.Types.Kinds.Decorators.Logic;

public sealed class KindingVehicleKind(VehicleKind kind) : VehicleKind(kind)
{
    public Vehicle KindVehicle(Vehicle vehicle) => new(vehicle, this);
}
