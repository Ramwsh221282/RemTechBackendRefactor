using RemTech.Result.Pattern;
using Vehicles.Domain.CategoryContext.Errors;

namespace Vehicles.Domain.CategoryContext.ValueObjects;

public readonly record struct CategoryId
{
    public Guid Value { get; }

    private CategoryId(Guid value) => Value = value;

    public CategoryId() => Value = Guid.NewGuid();

    public static Result<CategoryId> Create(Guid value) =>
        value == Guid.Empty ? new CategoryIdEmptyError() : new CategoryId(value);

    public static Result<CategoryId> Create(Guid? value) =>
        value == null ? new CategoryIdEmptyError() : new CategoryId(value.Value);
}
