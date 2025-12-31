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
        if (value.Length > MaxLength)
            return Error.Validation($"Имя локации не может быть больше {MaxLength} символов.");
        return new LocationName(value);
    }
}