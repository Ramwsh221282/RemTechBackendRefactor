using RemTech.Result.Pattern;
using Vehicles.Domain.BrandContext.Errors;

namespace Vehicles.Domain.BrandContext.ValueObjects;

public readonly record struct BrandOwnedVehiclesCount
{
    public long Value { get; }

    public BrandOwnedVehiclesCount() => Value = 0;

    private BrandOwnedVehiclesCount(long value) => Value = value;

    public BrandOwnedVehiclesCount Increase()
    {
        long nextValue = Value + 1;
        return new BrandOwnedVehiclesCount(nextValue);
    }

    public Result<BrandOwnedVehiclesCount> Decrease()
    {
        long nextValue = Value - 1;
        return Create(nextValue);
    }

    public static Result<BrandOwnedVehiclesCount> Create(long value) =>
        value < 0 ? new BrandNegativeVehiclesCountError() : new BrandOwnedVehiclesCount(value);

    public static Result<BrandOwnedVehiclesCount> Create(long? value) =>
        value == null
            ? new BrandEmptyVehiclesCountError()
            : new BrandOwnedVehiclesCount(value.Value);
}
