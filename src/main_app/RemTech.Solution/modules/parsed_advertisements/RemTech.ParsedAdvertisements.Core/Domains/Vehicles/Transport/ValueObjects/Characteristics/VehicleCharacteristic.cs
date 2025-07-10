using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

public sealed record VehicleCharacteristic
{
    private readonly ICharacteristic _characteristic;
    private readonly VehicleCharacteristicValue _value;

    public VehicleCharacteristic(ICharacteristic characteristic, VehicleCharacteristicValue value)
    {
        _characteristic = characteristic;
        _value = value;
    }

    public VehicleCharacteristicValue WhatValue() => _value;

    public ICharacteristic WhatCharacteristic() => _characteristic;
}
