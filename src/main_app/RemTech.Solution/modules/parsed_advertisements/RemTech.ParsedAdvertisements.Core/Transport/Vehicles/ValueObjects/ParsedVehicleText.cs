using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Transport.Vehicles.ValueObjects;

public sealed record ParsedVehicleText
{
    private readonly NotEmptyString _description;
    private readonly NotEmptyString _title;

    public ParsedVehicleText(NotEmptyString description, NotEmptyString title)
    {
        _description = description;
        _title = title;
    }

    public ParsedVehicleText(string? description, string? title)
    {
        _description = new NotEmptyString(description);
        _title = new NotEmptyString(title);
    }

    public static implicit operator bool(ParsedVehicleText text)
    {
        return text._description && text._title;
    }
}
