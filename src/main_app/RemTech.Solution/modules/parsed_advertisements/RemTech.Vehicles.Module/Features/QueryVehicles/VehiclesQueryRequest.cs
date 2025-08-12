using RemTech.Vehicles.Module.Features.QueryVehicles.Arguments;
using RemTech.Vehicles.Module.Features.QueryVehicles.Extensions;
using RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;

namespace RemTech.Vehicles.Module.Features.QueryVehicles;

public sealed record VehiclesQueryRequest(
    VehiclePaginationQueryFilterArgument? Pagination = null,
    VehicleKindIdQueryFilterArgument? KindId = null,
    VehicleBrandIdQueryFilterArgument? BrandId = null,
    VehicleModelIdQueryFilterArgument? ModelId = null,
    VehicleSortOrderQueryFilterArgument? SortOrder = null,
    VehicleRegionIdQueryFilterArgument? RegionId = null,
    VehiclePriceQueryFilterArgument? Price = null,
    VehicleTextSearchQueryFilterArgument? Text = null
) : VehicleQueryFilterArgument
{
    internal override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        CompositeVehicleSpeicification composite = new();
        composite = KindId.ApplyIfProvided(composite);
        composite = BrandId.ApplyIfProvided(composite);
        composite = ModelId.ApplyIfProvided(composite);
        composite = Pagination.ApplyIfProvided(composite);
        composite = RegionId.ApplyIfProvided(composite);
        composite = Price.ApplyIfProvided(composite);
        composite = SortOrder.ApplyIfProvided(composite);
        composite = RegionId.ApplyIfProvided(composite);
        composite = Price.ApplyIfProvided(composite);
        composite = Text.ApplyIfProvided(composite);
        return composite;
    }
}
