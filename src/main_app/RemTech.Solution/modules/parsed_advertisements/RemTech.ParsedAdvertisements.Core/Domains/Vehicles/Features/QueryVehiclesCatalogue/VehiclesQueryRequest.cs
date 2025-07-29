using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehicles.Arguments;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehicles.Extensions;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue;

public sealed record VehiclesQueryRequest(
    VehicleKindIdQueryFilterArgument KindId,
    VehicleBrandIdQueryFilterArgument BrandId,
    VehicleModelIdQueryFilterArgument ModelId,
    VehiclePaginationQueryFilterArgument Pagination,
    VehicleSortOrderQueryFilterArgument? SortOrder = null,
    VehicleRegionIdQueryFilterArgument? RegionId = null,
    VehiclePriceQueryFilterArgument? Price = null,
    VehicleCharacteristicsQueryArguments? Characteristics = null
) : VehicleQueryFilterArgument
{
    public override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        CompositeVehicleSpeicification composite = new();
        composite = KindId.ApplyTo(composite);
        composite = BrandId.ApplyTo(composite);
        composite = ModelId.ApplyTo(composite);
        composite = Pagination.ApplyTo(composite);
        composite = RegionId.ApplyIfProvided(composite);
        composite = Price.ApplyIfProvided(composite);
        composite = SortOrder.ApplyIfProvided(composite);
        composite = RegionId.ApplyIfProvided(composite);
        composite = Price.ApplyIfProvided(composite);
        composite = Characteristics.ApplyIfProvided(composite);
        return composite;
    }
}
