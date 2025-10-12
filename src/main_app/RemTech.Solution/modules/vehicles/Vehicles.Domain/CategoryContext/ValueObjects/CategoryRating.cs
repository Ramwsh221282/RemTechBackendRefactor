using RemTech.Result.Pattern;
using Vehicles.Domain.CategoryContext.Errors;

namespace Vehicles.Domain.CategoryContext.ValueObjects;

public readonly record struct CategoryRating
{
    public long Value { get; }

    public CategoryRating() => Value = 0;

    private CategoryRating(long value) => Value = value;

    public static Result<CategoryRating> Create(long value) =>
        value < 0 ? new CategoryRatingNegativeError() : new CategoryRating(value);

    public static Result<CategoryRating> Create(long? value) =>
        value == null ? new CategoryRatingEmptyError() : new CategoryRating(value.Value);
}
