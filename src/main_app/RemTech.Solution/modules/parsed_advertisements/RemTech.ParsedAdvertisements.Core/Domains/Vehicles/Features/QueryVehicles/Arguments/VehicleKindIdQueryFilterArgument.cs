using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Specifications;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Arguments;

public sealed record VehicleKindIdQueryFilterArgument(Guid Id) : VehicleQueryFilterArgument
{
    public override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        VehicleKindIdentity identity = new(
            new VehicleKindId(Id),
            new VehicleKindText(string.Empty)
        );
        return speicification.With(new VehicleKindQuerySpecification(identity));
    }
}