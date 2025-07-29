using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Arguments;

public sealed record VehiclePaginationQueryFilterArgument(int Page) : VehicleQueryFilterArgument
{
    public override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        return speicification.With(new VehiclePaginationQuerySpecification(Page));
    }
}
