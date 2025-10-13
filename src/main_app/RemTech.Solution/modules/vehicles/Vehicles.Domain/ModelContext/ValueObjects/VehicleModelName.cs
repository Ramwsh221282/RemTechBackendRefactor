using RemTech.Result.Pattern;
using Vehicles.Domain.ModelContext.Errors;

namespace Vehicles.Domain.ModelContext.ValueObjects;

public sealed record VehicleModelName
{
    public const int MaxLength = 80;
    public string Value { get; }

    private VehicleModelName(string value) => Value = value;

    public static Result<VehicleModelName> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new VehicleModelNameEmptyError();
        if (name.Length > MaxLength)
            return new VehicleModelNameExceesLengthError(MaxLength);
        return new VehicleModelName(name);
    }
}
