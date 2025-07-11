using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;

public readonly record struct CharacteristicId
{
    private readonly NotEmptyGuid _id;

    public CharacteristicId(Guid? id)
        : this(new NotEmptyGuid(id)) { }

    public CharacteristicId()
        : this(new NewGuid()) { }

    public CharacteristicId(NotEmptyGuid id)
    {
        _id = id;
    }

    public static implicit operator bool(CharacteristicId? id)
    {
        return id == null ? false : id.Value._id;
    }

    public static implicit operator Guid(CharacteristicId id) => id._id;

    public static implicit operator NotEmptyGuid(CharacteristicId id) => id._id;
}
