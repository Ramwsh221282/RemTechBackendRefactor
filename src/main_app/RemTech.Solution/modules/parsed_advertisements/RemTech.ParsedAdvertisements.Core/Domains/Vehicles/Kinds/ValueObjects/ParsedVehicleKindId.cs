using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;

public readonly record struct ParsedVehicleKindId
{
    private readonly NotEmptyGuid _id;

    public ParsedVehicleKindId()
    {
        _id = new NotEmptyGuid();
    }

    public ParsedVehicleKindId(NotEmptyGuid id)
    {
        _id = id;
    }

    public ParsedVehicleKindId(Guid id)
    {
        _id = new NotEmptyGuid(id);
    }

    public static implicit operator NotEmptyGuid(ParsedVehicleKindId id) => id._id;

    public static implicit operator Guid(ParsedVehicleKindId id) => id._id;

    public static implicit operator bool(ParsedVehicleKindId id) => id._id;
}
