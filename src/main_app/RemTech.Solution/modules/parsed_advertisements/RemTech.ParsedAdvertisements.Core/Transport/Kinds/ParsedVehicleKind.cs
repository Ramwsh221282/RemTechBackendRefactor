using RemTech.ParsedAdvertisements.Core.Transport.Kinds.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Transport.Vehicles;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Transport.Kinds;

public sealed class ParsedVehicleKind
{
    private readonly ParsedVehicleKindIdentity _identity;
    private ParsedVehicle[] _vehicles;

    public ParsedVehicleKind(ParsedVehicleKindText text)
        : this(new ParsedVehicleKindIdentity(text), []) { }

    public ParsedVehicleKind(ParsedVehicleKindId id, ParsedVehicleKindText text)
        : this(new ParsedVehicleKindIdentity(id, text), []) { }

    public ParsedVehicleKind(ParsedVehicleKindIdentity identity, ParsedVehicle[] vehicles)
    {
        _vehicles = vehicles;
        _identity = identity;
    }

    public Status<ParsedVehicle> PutVehicle(ParsedVehicle vehicle)
    {
        _vehicles = [.. _vehicles, vehicle];
        return vehicle;
    }

    public ParsedVehicleKindIdentity Identify() => _identity;
}
