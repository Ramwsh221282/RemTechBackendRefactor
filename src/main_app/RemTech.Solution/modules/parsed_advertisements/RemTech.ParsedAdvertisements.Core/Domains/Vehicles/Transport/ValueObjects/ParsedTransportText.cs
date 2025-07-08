using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;

public sealed record ParsedTransportText
{
    private readonly NotEmptyString _description;
    private readonly NotEmptyString _title;

    public ParsedTransportText(NotEmptyString description, NotEmptyString title)
    {
        _description = description;
        _title = title;
    }

    public ParsedTransportText(string? description, string? title)
    {
        _description = new NotEmptyString(description);
        _title = new NotEmptyString(title);
    }

    public static implicit operator bool(ParsedTransportText text)
    {
        return text._description && text._title;
    }
}
