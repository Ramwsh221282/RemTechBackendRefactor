using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Vehicles;

public sealed record VehicleTextInformation
{
    public string Value { get; }

    private VehicleTextInformation(string value)
    {
        Value = value;
    }

    public static Result<VehicleTextInformation> Create(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? (Result<VehicleTextInformation>)Error.Validation("Текстовая информация о технике не может быть пустой.")
            : (Result<VehicleTextInformation>)new VehicleTextInformation(value);
    }
}
