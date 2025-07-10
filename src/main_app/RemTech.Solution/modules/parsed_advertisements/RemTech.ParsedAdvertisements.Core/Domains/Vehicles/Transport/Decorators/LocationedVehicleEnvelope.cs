using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;

public sealed class LocationedVehicleEnvelope : VehicleEnvelope
{
    public LocationedVehicleEnvelope(NotEmptyGuid id, NotEmptyString text, IVehicle origin)
        : this(new ExistingGeoLocation(id, text), origin) { }

    public LocationedVehicleEnvelope(NotEmptyGuid id, NotEmptyString text)
        : this(new ExistingGeoLocation(id, text), new VehicleBlueprint()) { }

    public LocationedVehicleEnvelope(NotEmptyString text, IVehicle origin)
        : this(new NewGeoLocation(text), origin) { }

    public LocationedVehicleEnvelope(NotEmptyString text)
        : this(new NewGeoLocation(text), new VehicleBlueprint()) { }

    public LocationedVehicleEnvelope(Guid? id, string? text, IVehicle origin)
        : this(new NotEmptyGuid(id), new NotEmptyString(text), origin) { }

    public LocationedVehicleEnvelope(Guid? id, string? text)
        : this(new NotEmptyGuid(id), new NotEmptyString(text), new VehicleBlueprint()) { }

    public LocationedVehicleEnvelope(string? text, IVehicle origin)
        : this(new NotEmptyString(text), origin) { }

    public LocationedVehicleEnvelope(string? text)
        : this(new NotEmptyString(text), new VehicleBlueprint()) { }

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
