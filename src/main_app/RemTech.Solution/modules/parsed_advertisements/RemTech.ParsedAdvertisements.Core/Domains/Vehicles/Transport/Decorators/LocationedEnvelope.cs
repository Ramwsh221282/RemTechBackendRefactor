using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;

public sealed class LocationedEnvelope : VehicleEnvelope
{
    public LocationedEnvelope(NotEmptyGuid id, NotEmptyString text, IVehicle origin)
        : this(new ExistingGeoLocation(id, text), origin) { }

    public LocationedEnvelope(NotEmptyGuid id, NotEmptyString text)
        : this(new ExistingGeoLocation(id, text), new VehicleBlueprint()) { }

    public LocationedEnvelope(NotEmptyString text, IVehicle origin)
        : this(new NewGeoLocation(text), origin) { }

    public LocationedEnvelope(NotEmptyString text)
        : this(new NewGeoLocation(text), new VehicleBlueprint()) { }

    public LocationedEnvelope(Guid? id, string? text, IVehicle origin)
        : this(new NotEmptyGuid(id), new NotEmptyString(text), origin) { }

    public LocationedEnvelope(Guid? id, string? text)
        : this(new NotEmptyGuid(id), new NotEmptyString(text), new VehicleBlueprint()) { }

    public LocationedEnvelope(string? text, IVehicle origin)
        : this(new NotEmptyString(text), origin) { }

    public LocationedEnvelope(string? text)
        : this(new NotEmptyString(text), new VehicleBlueprint()) { }

    public LocationedEnvelope(IGeoLocation location, IVehicle origin)
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
