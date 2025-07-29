using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Types.Kinds.ValueObjects;

public readonly record struct VehicleKindId
{
    private readonly NotEmptyGuid _id;

    public VehicleKindId()
    {
        _id = new NewGuid();
    }

    public VehicleKindId(NotEmptyGuid id)
    {
        _id = id;
    }

    public VehicleKindId(Guid id)
    {
        _id = new NotEmptyGuid(id);
    }

    public static implicit operator NotEmptyGuid(VehicleKindId id) => id._id;

    public static implicit operator Guid(VehicleKindId id) => id._id;

    public static implicit operator bool(VehicleKindId? id)
    {
        return id != null && id.Value._id;
    }
}
