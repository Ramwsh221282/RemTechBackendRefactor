using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;
using RemTech.ParsedAdvertisements.Core.Types.GeoLocations.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicles.Arguments;

public sealed record VehicleRegionIdQueryFilterArgument(Guid Id) : VehicleQueryFilterArgument
{
    public override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        return speicification.With(
            new VehicleRegionQuerySpecification(
                new GeoLocationIdentity(
                    new GeoLocationId(new NotEmptyGuid(Id)),
                    new GeolocationText(string.Empty),
                    new GeolocationText(string.Empty)
                )
            )
        );
    }
}
