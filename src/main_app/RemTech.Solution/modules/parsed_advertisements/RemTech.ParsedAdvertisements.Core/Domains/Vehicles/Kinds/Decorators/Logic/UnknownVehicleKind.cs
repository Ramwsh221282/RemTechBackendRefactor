using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators.Logic;

public sealed class UnknownVehicleKind : VehicleKind
{
    public UnknownVehicleKind() : base(new VehicleKindIdentity())
    {
        
    }
}