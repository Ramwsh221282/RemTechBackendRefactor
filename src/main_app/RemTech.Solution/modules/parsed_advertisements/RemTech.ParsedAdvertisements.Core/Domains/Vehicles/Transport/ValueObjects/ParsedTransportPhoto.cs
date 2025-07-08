using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;

public sealed record ParsedTransportPhoto
{
    private readonly NotEmptyString _source;

    public ParsedTransportPhoto(NotEmptyString source)
    {
        _source = source;
    }

    public ParsedTransportPhoto(string? source)
        : this(new NotEmptyString(source)) { }

    public static implicit operator bool(ParsedTransportPhoto photo)
    {
        return photo._source;
    }

    public static implicit operator string(ParsedTransportPhoto photo)
    {
        return photo._source;
    }

    public static implicit operator NotEmptyString(ParsedTransportPhoto photo)
    {
        return photo._source;
    }
}
