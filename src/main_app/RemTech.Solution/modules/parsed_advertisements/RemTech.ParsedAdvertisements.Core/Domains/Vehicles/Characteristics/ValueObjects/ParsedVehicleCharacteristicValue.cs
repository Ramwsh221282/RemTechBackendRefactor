using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;

public sealed record ParsedVehicleCharacteristicValue
{
    private readonly NotEmptyString _value;

    public ParsedVehicleCharacteristicValue(NotEmptyString value)
    {
        _value = value;
    }

    public ParsedVehicleCharacteristicValue(string? value)
        : this(new NotEmptyString(value)) { }

    public static implicit operator NotEmptyString(ParsedVehicleCharacteristicValue value)
    {
        return value._value;
    }

    public static implicit operator string(ParsedVehicleCharacteristicValue value)
    {
        return value._value;
    }

    public static implicit operator bool(ParsedVehicleCharacteristicValue value)
    {
        return value._value;
    }
}
