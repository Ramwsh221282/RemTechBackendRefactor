using RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;

namespace RemTech.Vehicles.Module.Features.QueryVehicles.Arguments;

public sealed record VehicleSortOrderQueryFilterArgument(string Order) : VehicleQueryFilterArgument
{
    public override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        return speicification.With(new VehiclePriceSortQuerySpecification(Order));
    }
}
