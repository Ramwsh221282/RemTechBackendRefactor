using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;

public sealed class LocationedVehicleEnvelope : VehicleEnvelope
{
    public LocationedVehicleEnvelope(
        NotEmptyGuid id,
        NotEmptyString text,
        NotEmptyString kind,
        IVehicle origin
    )
        : this(new ExistingGeoLocation(id, text, kind), origin) { }

    public LocationedVehicleEnvelope(IGeoLocation location, IVehicle origin)
        : base(
            origin.Identity(),
            origin.Kind(),
            origin.Brand(),
            location,
            origin.Cost(),
            origin.TextInformation(),
            origin.Photos(),
            origin.Characteristics()
        ) { }
}
