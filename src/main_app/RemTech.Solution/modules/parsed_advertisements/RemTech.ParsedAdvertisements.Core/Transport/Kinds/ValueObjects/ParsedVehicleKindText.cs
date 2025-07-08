using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Transport.Kinds.ValueObjects;

public sealed record ParsedVehicleKindText
{
    private readonly NotEmptyString _text;

    public ParsedVehicleKindText(NotEmptyString text)
    {
        _text = text;
    }

    public ParsedVehicleKindText(string? text)
    {
        _text = new NotEmptyString(text);
    }

    public static implicit operator string(ParsedVehicleKindText text) => text._text;

    public static implicit operator NotEmptyString(ParsedVehicleKindText text) => text._text;
}
