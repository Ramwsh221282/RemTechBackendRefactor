using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;

public sealed record VehicleText
{
    private readonly NotEmptyString _description;
    private readonly NotEmptyString _title;

    public VehicleText(NotEmptyString description, NotEmptyString title)
    {
        _description = description;
        _title = title;
    }

    public VehicleText(string? description, string? title)
    {
        _description = new NotEmptyString(description);
        _title = new NotEmptyString(title);
    }

    public static implicit operator bool(VehicleText text)
    {
        return text._description && text._title;
    }

    public NotEmptyString ReadTitle() => _title;

    public NotEmptyString ReadDescription() => _description;
}
