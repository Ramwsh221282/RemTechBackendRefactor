using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;

public sealed class ParsedGeoLocation
{
    private readonly ParsedGeoLocationIdentity _identity;
    private ParsedTransport[] _vehicles;

    public ParsedGeoLocation(ParsedGeoLocationIdentity identity, ParsedTransport[] vehicles)
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
        ParsedTransport[] vehicles
    )
        : this(new ParsedGeoLocationIdentity(id, text), vehicles) { }

    public Status<ParsedTransport> Put(ParsedTransport transport)
    {
        _vehicles = [.. _vehicles, transport];
        return transport;
    }

    public ParsedGeoLocationIdentity Identify() => _identity;
}
