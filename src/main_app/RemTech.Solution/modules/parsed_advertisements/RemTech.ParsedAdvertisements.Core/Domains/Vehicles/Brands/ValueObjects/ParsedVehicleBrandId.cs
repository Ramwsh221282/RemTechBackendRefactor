using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;

public readonly record struct ParsedVehicleBrandId
{
    private readonly NotEmptyGuid _id;

    public ParsedVehicleBrandId()
    {
        _id = new NotEmptyGuid();
    }

    public ParsedVehicleBrandId(NotEmptyGuid id)
    {
        _id = id;
    }

    public ParsedVehicleBrandId(Guid? id)
    {
        _id = new NotEmptyGuid(id);
    }

    public static implicit operator NotEmptyGuid(ParsedVehicleBrandId id) => id._id;

    public static implicit operator Guid(ParsedVehicleBrandId id) => id._id;

    public static implicit operator bool(ParsedVehicleBrandId id) => id._id;
}
