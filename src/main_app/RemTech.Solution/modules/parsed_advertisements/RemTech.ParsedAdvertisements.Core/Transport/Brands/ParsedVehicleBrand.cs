using RemTech.ParsedAdvertisements.Core.Transport.Brands.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Transport.Vehicles;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Transport.Brands;

public sealed class ParsedVehicleBrand
{
    private readonly ParsedVehicleBrandIdentity _identity;
    private ParsedVehicle[] _vehicles;

    public ParsedVehicleBrand(ParsedVehicleBrandText text)
        : this(new ParsedVehicleBrandIdentity(text), []) { }

    public ParsedVehicleBrand(ParsedVehicleBrandId id, ParsedVehicleBrandText text)
        : this(new ParsedVehicleBrandIdentity(id, text), []) { }

    public ParsedVehicleBrand(ParsedVehicleBrand brand, ParsedVehicle vehicle)
        : this(brand._identity, [.. brand._vehicles, vehicle]) { }

    public ParsedVehicleBrand(ParsedVehicleBrandIdentity identity, ParsedVehicle[] vehicles)
    {
        _vehicles = vehicles;
        _identity = identity;
    }

    public Status<ParsedVehicle> PutVehicle(ParsedVehicle vehicle)
    {
        _vehicles = [.. _vehicles, vehicle];
        return vehicle;
    }

    public ParsedVehicleBrandIdentity Identify() => _identity;
}
