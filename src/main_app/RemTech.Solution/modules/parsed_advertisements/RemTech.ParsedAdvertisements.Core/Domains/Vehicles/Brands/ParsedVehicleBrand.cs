using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;

public sealed class ParsedVehicleBrand
{
    private readonly ParsedVehicleBrandIdentity _identity;
    private ParsedTransport[] _vehicles;

    public ParsedVehicleBrand(ParsedVehicleBrandText text)
        : this(new ParsedVehicleBrandIdentity(text), []) { }

    public ParsedVehicleBrand(ParsedVehicleBrandId id, ParsedVehicleBrandText text)
        : this(new ParsedVehicleBrandIdentity(id, text), []) { }

    public ParsedVehicleBrand(ParsedVehicleBrand brand, ParsedTransport transport)
        : this(brand._identity, [.. brand._vehicles, transport]) { }

    public ParsedVehicleBrand(ParsedVehicleBrandIdentity identity, ParsedTransport[] vehicles)
    {
        _vehicles = vehicles;
        _identity = identity;
    }

    public Status<ParsedTransport> PutVehicle(ParsedTransport transport)
    {
        _vehicles = [.. _vehicles, transport];
        return transport;
    }

    public ParsedVehicleBrandIdentity Identify() => _identity;
}
