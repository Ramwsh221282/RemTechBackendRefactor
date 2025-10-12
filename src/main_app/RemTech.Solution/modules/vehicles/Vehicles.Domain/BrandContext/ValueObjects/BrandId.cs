using RemTech.Result.Pattern;
using Vehicles.Domain.BrandContext.Errors;

namespace Vehicles.Domain.BrandContext.ValueObjects;

public readonly record struct BrandId
{
    public Guid Id { get; }

    public BrandId() => Id = Guid.NewGuid();

    private BrandId(Guid id) => Id = id;

    public static Result<BrandId> Create(Guid value) =>
        value == Guid.Empty ? new BrandIdEmptyError() : new BrandId(value);

    public static Result<BrandId> Create(Guid? value) =>
        value == null ? new BrandIdEmptyError() : Create(value.Value);
}
