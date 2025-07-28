using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Specifications;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Arguments;

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