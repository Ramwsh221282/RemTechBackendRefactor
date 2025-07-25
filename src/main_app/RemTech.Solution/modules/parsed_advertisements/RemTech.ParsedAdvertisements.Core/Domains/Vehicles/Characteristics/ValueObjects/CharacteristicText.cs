using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;

public sealed record CharacteristicText
{
    private readonly NotEmptyString _text;

    public CharacteristicText(NotEmptyString text)
    {
        _text = text;
    }

    public CharacteristicText(string? text)
        : this(new NotEmptyString(text)) { }

    public static implicit operator bool(CharacteristicText? text)
    {
        return text != null && text._text;
    }

    public static implicit operator NotEmptyString(CharacteristicText text) => text._text;

    public static implicit operator string(CharacteristicText text) => text._text;
}
