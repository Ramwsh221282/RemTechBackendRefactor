using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Transport.Characteristics.ValueObjects;

public sealed record ParsedVehicleCharacteristicText
{
    private readonly NotEmptyString _text;

    public ParsedVehicleCharacteristicText(NotEmptyString text)
    {
        _text = text;
    }

    public ParsedVehicleCharacteristicText(string? text)
        : this(new NotEmptyString(text)) { }

    public static implicit operator bool(ParsedVehicleCharacteristicText text) => text._text;

    public static implicit operator NotEmptyString(ParsedVehicleCharacteristicText text) =>
        text._text;

    public static implicit operator string(ParsedVehicleCharacteristicText text) => text._text;
}
