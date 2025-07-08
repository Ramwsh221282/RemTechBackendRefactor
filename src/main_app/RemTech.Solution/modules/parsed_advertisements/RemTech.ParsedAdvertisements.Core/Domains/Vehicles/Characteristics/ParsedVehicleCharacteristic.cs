using System.Runtime.InteropServices.ComTypes;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;

public sealed class ParsedVehicleCharacteristic
{
    private readonly ParsedVehicleCharacteristicIdentity _identity;
    private readonly ParsedVehicleCharacteristicValue _value;

    public ParsedVehicleCharacteristic(
        ParsedVehicleCharacteristicText text,
        ParsedVehicleCharacteristicValue value
    )
        : this(new ParsedVehicleCharacteristicIdentity(text), value) { }

    public ParsedVehicleCharacteristic(
        ParsedVehicleCharacteristicId id,
        ParsedVehicleCharacteristicText text,
        ParsedVehicleCharacteristicValue value
    )
        : this(new ParsedVehicleCharacteristicIdentity(id, text), value) { }

    public ParsedVehicleCharacteristic(
        ParsedVehicleCharacteristicIdentity identity,
        ParsedVehicleCharacteristicValue value
    )
    {
        _identity = identity;
        _value = value;
    }

    public ParsedVehicleCharacteristicIdentity Identify() => _identity;

    public static implicit operator string(ParsedVehicleCharacteristic ctx)
    {
        return ctx._value;
    }
}
