using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Vehicles.Module.Types.Characteristics;

namespace RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

public sealed record VehicleCharacteristic
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

    public string NameValued() => _characteristic.NameValueString(_value);

    public ParametrizingPgCommand CtxPgCommand(int index, ParametrizingPgCommand cmd)
    {
        return _characteristic.VehicleCtxPgCommand(_value, index, cmd);
    }
}
