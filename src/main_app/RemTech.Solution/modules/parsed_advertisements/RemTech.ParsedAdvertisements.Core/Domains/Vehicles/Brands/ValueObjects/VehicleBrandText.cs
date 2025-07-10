using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;

public sealed record VehicleBrandText
{
    private readonly NotEmptyString _text;

    public VehicleBrandText(NotEmptyString text)
    {
        _text = text;
    }

    public VehicleBrandText(string? text)
    {
        _text = new NotEmptyString(text);
    }

    public static implicit operator bool(VehicleBrandText? text)
    {
        return text == null ? false : text._text;
    }

    public static implicit operator NotEmptyString(VehicleBrandText text) => text._text;

    public static implicit operator string(VehicleBrandText text) => text._text;
}
