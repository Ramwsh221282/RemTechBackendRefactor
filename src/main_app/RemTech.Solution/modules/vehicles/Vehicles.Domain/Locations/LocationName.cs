using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Locations;

public sealed record LocationName
{
    private const int MaxLength = 255;
    public string Value { get; }

    private LocationName(string value)
    {
        Value = value;
    }

    public static Result<LocationName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("Имя локации не может быть пустым.");
        return value.Length > MaxLength
            ? (Result<LocationName>)Error.Validation($"Имя локации не может быть больше {MaxLength} символов.")
            : (Result<LocationName>)new LocationName(value);
    }
}
