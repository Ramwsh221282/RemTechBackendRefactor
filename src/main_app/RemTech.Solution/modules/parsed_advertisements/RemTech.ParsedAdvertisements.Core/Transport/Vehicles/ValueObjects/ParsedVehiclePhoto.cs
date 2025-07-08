using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Transport.Vehicles.ValueObjects;

public sealed record ParsedVehiclePhoto
{
    private readonly NotEmptyString _source;

    public ParsedVehiclePhoto(NotEmptyString source)
    {
        _source = source;
    }

    public ParsedVehiclePhoto(string? source)
        : this(new NotEmptyString(source)) { }

    public static implicit operator bool(ParsedVehiclePhoto photo)
    {
        return photo._source;
    }

    public static implicit operator string(ParsedVehiclePhoto photo)
    {
        return photo._source;
    }

    public static implicit operator NotEmptyString(ParsedVehiclePhoto photo)
    {
        return photo._source;
    }
}
