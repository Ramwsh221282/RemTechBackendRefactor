using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.CharacteristicContext.ValueObjects;

public sealed record CharacteristicName
{
    public const int MaxLength = 100;
    public string Value { get; }

    private CharacteristicName(string value) => Value = value;

    public static Status<CharacteristicName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("Характеристика техники не может быть пустой.");

        if (value.Length > MaxLength)
            return Error.Validation(
                $"Характеристика техники не может превышать {MaxLength} символов."
            );

        return new CharacteristicName(value);
    }
}
