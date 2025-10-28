using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.RegionContext.ValueObjects;

public readonly record struct RegionId
{
    public Guid Value { get; }

    public RegionId() => Value = Guid.NewGuid();

    private RegionId(Guid value) => Value = value;

    public static Status<RegionId> Create(Guid value)
    {
        if (value == Guid.Empty)
            return Error.Validation("Идентификатор региона не может быть пустым.");
        return new RegionId(value);
    }
}
