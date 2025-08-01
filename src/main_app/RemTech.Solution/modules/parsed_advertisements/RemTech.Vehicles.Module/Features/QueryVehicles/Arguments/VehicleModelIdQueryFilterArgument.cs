using RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;
using RemTech.Vehicles.Module.Types.Models.ValueObjects;

namespace RemTech.Vehicles.Module.Features.QueryVehicles.Arguments;

public sealed record VehicleModelIdQueryFilterArgument(Guid Id) : VehicleQueryFilterArgument
{
    public override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        return speicification.With(
            new VehicleModelQuerySpecification(new VehicleModelIdentity(Id))
        );
    }
}
