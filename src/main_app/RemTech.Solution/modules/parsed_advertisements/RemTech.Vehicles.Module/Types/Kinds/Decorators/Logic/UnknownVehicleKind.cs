using RemTech.Vehicles.Module.Types.Kinds.ValueObjects;

namespace RemTech.Vehicles.Module.Types.Kinds.Decorators.Logic;

public sealed class UnknownVehicleKind : VehicleKind
{
    public UnknownVehicleKind()
        : base(new VehicleKindIdentity()) { }
}
