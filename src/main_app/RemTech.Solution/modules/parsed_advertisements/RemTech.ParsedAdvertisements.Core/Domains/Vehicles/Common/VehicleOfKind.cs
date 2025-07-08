using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Common;

public sealed class VehicleOfKind
{
    private readonly ParsedTransport _transport;
    private readonly ParsedVehicleKind _kind;

    public VehicleOfKind(ParsedTransport transport, ParsedVehicleKind kind)
    {
        _transport = transport;
        _kind = kind;
    }

    public ParsedTransport WhatTransport() => _transport;

    public ParsedVehicleKind WhatKind() => _kind;

    public static implicit operator ParsedTransport(VehicleOfKind vok)
    {
        return vok._transport;
    }

    public static implicit operator ParsedVehicleKind(VehicleOfKind vok)
    {
        return vok._kind;
    }
}
