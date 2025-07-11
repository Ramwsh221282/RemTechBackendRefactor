using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;

public readonly record struct GeoLocationId
{
    public readonly NotEmptyGuid _id;

    public GeoLocationId()
    {
        _id = new NewGuid();
    }

    public GeoLocationId(NotEmptyGuid id)
    {
        _id = id;
    }

    public static implicit operator bool(GeoLocationId? id)
    {
        return id != null && id.Value._id;
    }

    public static implicit operator Guid(GeoLocationId id) => id._id;
}
