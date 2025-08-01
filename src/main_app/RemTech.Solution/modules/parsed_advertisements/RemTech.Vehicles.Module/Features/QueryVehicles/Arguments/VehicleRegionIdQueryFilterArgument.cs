using RemTech.Core.Shared.Primitives;
using RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;
using RemTech.Vehicles.Module.Types.GeoLocations.ValueObjects;

namespace RemTech.Vehicles.Module.Features.QueryVehicles.Arguments;

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
