using RemTech.Result.Pattern;
using Vehicles.Domain.VehicleContext.Errors;

namespace Vehicles.Domain.VehicleContext.ValueObjects;

public sealed record VehicleDescription
{
    public const int MaxLength = 500;
    public string Value { get; }

    private VehicleDescription(string value) => Value = value;

    public static Result<VehicleDescription> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new VehicleDescriptionEmptyError();
        if (value.Length > MaxLength)
            return new VehicleDescriptionExceesLengthError(MaxLength);
        return new VehicleDescription(value);
    }
}
