using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Types.Brands.ValueObjects;

public readonly record struct VehicleBrandId
{
    private readonly NotEmptyGuid _id;

    public VehicleBrandId()
    {
        _id = new NewGuid();
    }

    public VehicleBrandId(NotEmptyGuid id)
    {
        _id = id;
    }

    public VehicleBrandId(Guid? id)
    {
        _id = new NotEmptyGuid(id);
    }

    public static implicit operator NotEmptyGuid(VehicleBrandId id) => id._id;

    public static implicit operator Guid(VehicleBrandId id) => id._id;

    public static implicit operator bool(VehicleBrandId id) => id._id;

    public static implicit operator bool(VehicleBrandId? id)
    {
        return id == null ? false : id.Value._id;
    }
}
