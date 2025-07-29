using RemTech.Core.Shared.Primitives;

namespace RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

public sealed record VehicleCharacteristicValue
{
    private readonly NotEmptyString _value;

    public VehicleCharacteristicValue(NotEmptyString value)
    {
        _value = value;
    }

    public VehicleCharacteristicValue(string? value)
        : this(new NotEmptyString(value)) { }

    public static implicit operator NotEmptyString(VehicleCharacteristicValue value)
    {
        return value._value;
    }

    public static implicit operator string(VehicleCharacteristicValue value)
    {
        return value._value;
    }

    public static implicit operator bool(VehicleCharacteristicValue value)
    {
        return value._value;
    }
}
