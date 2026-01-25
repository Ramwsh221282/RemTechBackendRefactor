using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Vehicles;

public sealed record VehicleCharacteristicValue
{
    public string Value { get; }

    private VehicleCharacteristicValue(string value)
    {
        Value = value;
    }

    public static Result<VehicleCharacteristicValue> Create(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? (Result<VehicleCharacteristicValue>)Error.Validation("Значение характеристики не может быть пустым.")
            : (Result<VehicleCharacteristicValue>)new VehicleCharacteristicValue(value);
    }
}
