using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Characteristics;

public sealed record CharacteristicName
{
    private const int MaxLength = 128;
    public string Value { get; }

    private CharacteristicName(string value)
    {
        Value = value;
    }

    public static Result<CharacteristicName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("Имя характеристики не может быть пустым.");
        return value.Length > MaxLength
            ? (Result<CharacteristicName>)Error.Validation($"Имя характеристики не может быть больше {MaxLength} символов.")
            : (Result<CharacteristicName>)new CharacteristicName(value);
    }
}
