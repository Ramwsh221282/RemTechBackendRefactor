using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;

public readonly record struct ParsedVehicleCharacteristicId
{
    private readonly NotEmptyGuid _id;

    public ParsedVehicleCharacteristicId(Guid? id)
        : this(new NotEmptyGuid(id)) { }

    public ParsedVehicleCharacteristicId()
        : this(new NotEmptyGuid()) { }

    public ParsedVehicleCharacteristicId(NotEmptyGuid id)
    {
        _id = id;
    }

    public static implicit operator bool(ParsedVehicleCharacteristicId id) => id._id;

    public static implicit operator Guid(ParsedVehicleCharacteristicId id) => id._id;

    public static implicit operator NotEmptyGuid(ParsedVehicleCharacteristicId id) => id._id;
}
