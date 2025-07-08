using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;

public sealed class ParsedVehicleKind
{
    private readonly ParsedVehicleKindIdentity _identity;
    private ParsedTransport[] _vehicles;

    public ParsedVehicleKind(ParsedVehicleKindText text)
        : this(new ParsedVehicleKindIdentity(text), []) { }

    public ParsedVehicleKind(ParsedVehicleKindId id, ParsedVehicleKindText text)
        : this(new ParsedVehicleKindIdentity(id, text), []) { }

    public ParsedVehicleKind(ParsedVehicleKindIdentity identity, ParsedTransport[] vehicles)
    {
        _vehicles = vehicles;
        _identity = identity;
    }

    public Status<ParsedTransport> PutVehicle(ParsedTransport transport)
    {
        _vehicles = [.. _vehicles, transport];
        return transport;
    }

    public ParsedVehicleKindIdentity Identify() => _identity;
}
