using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.CharacteristicContext.ValueObjects;

public readonly record struct CharacteristicId
{
    public Guid Value { get; }

    public CharacteristicId() => Value = Guid.NewGuid();

    private CharacteristicId(Guid value) => Value = value;

    public static Status<CharacteristicId> Create(Guid value)
    {
        if (value == Guid.Empty)
            return Error.Validation("Характеристика техники не может быть пустой.");
        return new CharacteristicId(value);
    }
}
