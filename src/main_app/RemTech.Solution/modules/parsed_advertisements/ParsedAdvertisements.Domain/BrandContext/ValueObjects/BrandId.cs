using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.BrandContext.ValueObjects;

public readonly record struct BrandId
{
    public Guid Id { get; }

    public BrandId() => Id = Guid.NewGuid();

    public BrandId(Guid id) => Id = id;

    public static Status<BrandId> Create(Guid id)
    {
        if (id == Guid.Empty)
            return Error.Validation("Идентификатор бренда техники был пустым.");
        return new BrandId(id);
    }
}
