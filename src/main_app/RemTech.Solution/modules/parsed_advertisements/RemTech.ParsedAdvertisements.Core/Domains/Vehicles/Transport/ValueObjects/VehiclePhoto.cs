using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;

public sealed record VehiclePhoto
{
    private readonly NotEmptyString _source;

    public VehiclePhoto(NotEmptyString source)
    {
        _source = source;
    }

    public VehiclePhoto(string? source)
        : this(new NotEmptyString(source)) { }

    public static implicit operator bool(VehiclePhoto photo)
    {
        return photo._source;
    }

    public static implicit operator string(VehiclePhoto photo)
    {
        return photo._source;
    }

    public static implicit operator NotEmptyString(VehiclePhoto photo)
    {
        return photo._source;
    }
}
