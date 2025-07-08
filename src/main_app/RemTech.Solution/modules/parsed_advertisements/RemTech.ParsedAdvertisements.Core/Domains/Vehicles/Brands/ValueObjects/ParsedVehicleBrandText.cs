using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;

public sealed record ParsedVehicleBrandText
{
    private readonly NotEmptyString _text;

    public ParsedVehicleBrandText(NotEmptyString text)
    {
        _text = text;
    }

    public ParsedVehicleBrandText(string? text)
    {
        _text = new NotEmptyString(text);
    }

    public static implicit operator bool(ParsedVehicleBrandText text) => text._text;

    public static implicit operator NotEmptyString(ParsedVehicleBrandText text) => text._text;

    public static implicit operator string(ParsedVehicleBrandText text) => text._text;
}
