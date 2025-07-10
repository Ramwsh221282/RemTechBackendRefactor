using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;

public sealed record VehicleId
{
    private readonly NotEmptyString _id;

    public VehicleId(NotEmptyString id)
    {
        _id = id;
    }

    public VehicleId(string? id)
        : this(new NotEmptyString(id)) { }

    public static implicit operator string(VehicleId id) => id._id;

    public static implicit operator bool(VehicleId? id)
    {
        return id != null && id._id;
    }

    public static implicit operator NotEmptyString(VehicleId id) => id._id;
}
