using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;

public readonly record struct ParsedGeolocationId
{
    public readonly NotEmptyGuid _id;

    public ParsedGeolocationId()
    {
        _id = new NotEmptyGuid();
    }

    public ParsedGeolocationId(NotEmptyGuid id)
    {
        _id = id;
    }

    public static implicit operator bool(ParsedGeolocationId id) => id._id;

    public static implicit operator Guid(ParsedGeolocationId id) => id._id;
}
