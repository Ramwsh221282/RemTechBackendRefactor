using RemTech.ParsedAdvertisements.Core.Types.Kinds.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Types.Kinds.Decorators.Logic;

public sealed class UnknownVehicleKind : VehicleKind
{
    public UnknownVehicleKind()
        : base(new VehicleKindIdentity()) { }
}
