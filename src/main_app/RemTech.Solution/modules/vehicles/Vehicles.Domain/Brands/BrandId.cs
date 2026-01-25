using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Brands;

public readonly record struct BrandId
{
    public Guid Id { get; }

    public BrandId()
    {
        Id = Guid.NewGuid();
    }

    private BrandId(Guid id)
    {
        Id = id;
    }

    public static Result<BrandId> Create(Guid id)
    {
        return id == Guid.Empty ? (Result<BrandId>)Error.Validation("Идентификатор бренда не может быть пустым.") : (Result<BrandId>)new BrandId(id);
    }
}
