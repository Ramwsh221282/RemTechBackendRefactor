using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Common;

public sealed class VehicleCharacteristic
{
    private readonly ParsedTransport _transport;
    private readonly ParsedVehicleCharacteristic _characteristic;

    public VehicleCharacteristic(
        ParsedTransport transport,
        ParsedVehicleCharacteristic characteristic
    )
    {
        _transport = transport;
        _characteristic = characteristic;
    }

    public ParsedTransport WhatTransport() => _transport;

    public ParsedVehicleCharacteristic WhatCharacteristic() => _characteristic;

    public static implicit operator ParsedTransport(VehicleCharacteristic vc)
    {
        return vc._transport;
    }

    public static implicit operator ParsedVehicleCharacteristic(VehicleCharacteristic vc)
    {
        return vc._characteristic;
    }
}
