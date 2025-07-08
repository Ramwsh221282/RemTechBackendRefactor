using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Common;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;

public sealed class ParsedGeoLocation
{
    private readonly ParsedGeoLocationIdentity _identity;
    private VehicleOfGeo[] _vehicles;

    public ParsedGeoLocation(ParsedGeoLocationIdentity identity, VehicleOfGeo[] vehicles)
    {
        _identity = identity;
        _vehicles = vehicles;
    }

    public ParsedGeoLocation(ParsedGeolocationText text)
        : this(new ParsedGeoLocationIdentity(text), []) { }

    public ParsedGeoLocation(ParsedGeolocationId id, ParsedGeolocationText text)
        : this(new ParsedGeoLocationIdentity(id, text), []) { }

    public ParsedGeoLocation(
        ParsedGeolocationId id,
        ParsedGeolocationText text,
        VehicleOfGeo[] vehicles
    )
        : this(new ParsedGeoLocationIdentity(id, text), vehicles) { }

    public ParsedGeoLocationIdentity Identify() => _identity;

    public VehicleOfGeo PutGeoMark(ParsedTransport parsedTransport)
    {
        VehicleOfGeo vehicle = new(parsedTransport, this);
        _vehicles = [.. _vehicles, vehicle];
        return vehicle;
    }
}
