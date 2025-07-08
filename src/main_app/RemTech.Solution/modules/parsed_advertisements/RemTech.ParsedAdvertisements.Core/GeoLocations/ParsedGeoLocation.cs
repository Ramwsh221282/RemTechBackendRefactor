using RemTech.ParsedAdvertisements.Core.GeoLocations.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Transport.Vehicles;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.GeoLocations;

public sealed class ParsedGeoLocation
{
    private readonly ParsedGeoLocationIdentity _identity;
    private ParsedVehicle[] _vehicles;

    public ParsedGeoLocation(ParsedGeoLocationIdentity identity, ParsedVehicle[] vehicles)
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
        ParsedVehicle[] vehicles
    )
        : this(new ParsedGeoLocationIdentity(id, text), vehicles) { }

    public Status<ParsedVehicle> Put(ParsedVehicle vehicle)
    {
        _vehicles = [.. _vehicles, vehicle];
        return vehicle;
    }

    public ParsedGeoLocationIdentity Identify() => _identity;
}
