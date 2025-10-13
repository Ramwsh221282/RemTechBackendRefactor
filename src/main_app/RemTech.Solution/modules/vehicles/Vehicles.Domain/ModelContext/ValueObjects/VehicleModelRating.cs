using RemTech.Result.Pattern;
using Vehicles.Domain.ModelContext.Errors;

namespace Vehicles.Domain.ModelContext.ValueObjects;

public readonly record struct VehicleModelRating
{
    public long Value { get; }

    public VehicleModelRating() => Value = 0;

    private VehicleModelRating(long value) => Value = value;

    public static Result<VehicleModelRating> Create(long value) =>
        value < 0 ? new VehicleModelRatingNegativeError() : new VehicleModelRating(value);

    public static Result<VehicleModelRating> Create(long? value) =>
        value == null ? new VehicleModelRatingEmptyError() : Create(value.Value);
}
