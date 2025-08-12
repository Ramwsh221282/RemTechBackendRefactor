using RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;

namespace RemTech.Vehicles.Module.Features.QueryVehicles.Arguments;

public sealed record VehiclePaginationQueryFilterArgument(int Page) : VehicleQueryFilterArgument
{
    internal override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        return speicification.With(new VehiclePaginationQuerySpecification(Page));
    }
}
