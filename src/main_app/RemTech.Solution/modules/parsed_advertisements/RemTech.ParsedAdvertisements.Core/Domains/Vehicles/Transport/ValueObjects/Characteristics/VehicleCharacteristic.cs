using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

public sealed record VehicleCharacteristic
{
    private readonly CharacteristicIdentity _characteristic;
    private readonly VehicleCharacteristicValue _value;

    public VehicleCharacteristic(
        CharacteristicIdentity characteristic,
        VehicleCharacteristicValue value
    )
    {
        _characteristic = characteristic;
        _value = value;
    }

    public VehicleCharacteristicValue WhatValue() => _value;

    public CharacteristicIdentity WhatCharacteristics() => _characteristic;
}
