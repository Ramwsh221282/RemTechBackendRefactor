using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Common;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;

public sealed class ParsedVehicleKind
{
    private readonly ParsedVehicleKindIdentity _identity;
    private VehicleOfKind[] _vehicles;

    public ParsedVehicleKind(ParsedVehicleKindText text)
        : this(new ParsedVehicleKindIdentity(text), []) { }

    public ParsedVehicleKind(ParsedVehicleKindId id, ParsedVehicleKindText text)
        : this(new ParsedVehicleKindIdentity(id, text), []) { }

    public ParsedVehicleKind(ParsedVehicleKindIdentity identity, VehicleOfKind[] vehicles)
    {
        _vehicles = vehicles;
        _identity = identity;
    }

    public VehicleOfKind PutKindMark(ParsedTransport transport)
    {
        VehicleOfKind vehicle = new(transport, this);
        _vehicles = [.. _vehicles, vehicle];
        return vehicle;
    }

    public ParsedVehicleKindIdentity Identify() => _identity;
}
