using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Specifications;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Arguments;

public sealed record VehiclePaginationQueryFilterArgument(int Page) : VehicleQueryFilterArgument
{
    public override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        return speicification.With(new VehiclePaginationQuerySpecification(Page));
    }
}