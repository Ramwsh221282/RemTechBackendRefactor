using RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;

namespace RemTech.Vehicles.Module.Features.QueryVehicles.Arguments;

public sealed record VehiclePriceQueryFilterArgument(
    bool? IsNds = null,
    double? PriceFrom = null,
    double? PriceTo = null
) : VehicleQueryFilterArgument
{
    public override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        return speicification.With(new VehiclePriceQuerySpecification(PriceTo, PriceFrom, IsNds));
    }
}
