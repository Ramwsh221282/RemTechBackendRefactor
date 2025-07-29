using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;
using RemTech.Vehicles.Module.Types.Kinds.ValueObjects;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Arguments;

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
