using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Specifications;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Arguments;

public sealed record VehicleSortOrderQueryFilterArgument(string Order) : VehicleQueryFilterArgument
{
    public override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        return speicification.With(new VehiclePriceSortQuerySpecification(Order));
    }
}