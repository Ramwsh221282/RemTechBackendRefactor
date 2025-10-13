using RemTech.Result.Pattern;
using Vehicles.Domain.LocationContext.Errors;

namespace Vehicles.Domain.LocationContext.ValueObjects;

public readonly record struct LocationRating
{
    public long Value { get; }

    public LocationRating() => Value = 0;

    private LocationRating(long value) => Value = value;

    public static Result<LocationRating> Create(long value) =>
        value < 0 ? new LocationRatingNegativeError() : new LocationRating(value);

    public static Result<LocationRating> Create(long? value) =>
        value == null ? new LocationRatingEmptyError() : new LocationRating(value.Value);
}
