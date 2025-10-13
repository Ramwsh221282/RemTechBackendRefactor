using RemTech.Result.Pattern;
using Vehicles.Domain.VehicleContext.Errors;

namespace Vehicles.Domain.VehicleContext.ValueObjects;

public sealed record VehicleCharacteristic
{
    public const int MaxLength = 100;
    public string Name { get; }
    public string Value { get; }

    private VehicleCharacteristic(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public static Result<VehicleCharacteristic> Create(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new VehicleCharacteristicNameEmptyError();
        if (string.IsNullOrWhiteSpace(value))
            return new VehicleCharacteristicValueEmptyError();
        if (name.Length > MaxLength)
            return new VehicleCharacteristicNameExceesLengthError(MaxLength);
        if (value.Length > MaxLength)
            return new VehicleCharacteristicValueExceesLengthError(MaxLength);
        return new VehicleCharacteristic(name, value);
    }
}
