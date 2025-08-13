using RemTech.Vehicles.Module.Types.Characteristics;

namespace RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

internal sealed record VehicleCharacteristic
{
    private readonly Characteristic _characteristic;

    private readonly VehicleCharacteristicValue _value;

    public VehicleCharacteristic(Characteristic characteristic, VehicleCharacteristicValue value)
    {
        _characteristic = characteristic;
        _value = value;
    }

    public VehicleCharacteristicValue WhatValue() => _value;

    public Characteristic WhatCharacteristic() => _characteristic;
}
