using RemTech.Vehicles.Module.Types.Characteristics;
using Shared.Infrastructure.Module.Postgres.PgCommands;

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

    public PgVehicleCharacteristic MakePgCharacteristic()
    {
        Guid ctxId = _characteristic.Identity.ReadId();
        string ctxName = _characteristic.Identity.ReadText();
        string ctxMeasure = _characteristic.Measure();
        string ctxValue = _value;
        return new PgVehicleCharacteristic(ctxId, ctxName, ctxValue, ctxMeasure);
    }
}
