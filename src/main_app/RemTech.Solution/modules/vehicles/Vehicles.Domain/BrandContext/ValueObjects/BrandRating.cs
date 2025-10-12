using RemTech.Result.Pattern;
using Vehicles.Domain.BrandContext.Errors;

namespace Vehicles.Domain.BrandContext.ValueObjects;

public readonly record struct BrandRating
{
    public long Value { get; }

    public BrandRating() => Value = 0;

    private BrandRating(long value) => Value = value;

    public static Result<BrandRating> Create(long value) =>
        value < 0 ? new BrandNegativeRatingError() : new BrandRating(value);

    public static Result<BrandRating> Create(long? value) =>
        value == null ? new BrandRatingEmptyError() : Create(value.Value);
}
