using RemTech.Result.Pattern;
using Vehicles.Domain.CategoryContext.Errors;

namespace Vehicles.Domain.CategoryContext.ValueObjects;

public readonly record struct CategoryOwnedVehiclesCount
{
    public long Value { get; }

    public CategoryOwnedVehiclesCount() => Value = 0;

    private CategoryOwnedVehiclesCount(long value) => Value = value;

    public static Result<CategoryOwnedVehiclesCount> Create(long value) =>
        value < 0
            ? new CategoryOwnedVehiclesCountNegativeError()
            : new CategoryOwnedVehiclesCount(value);

    public static Result<CategoryOwnedVehiclesCount> Create(long? value) =>
        value == null
            ? new CategoryOwnedVehiclesCountEmptyError()
            : new CategoryOwnedVehiclesCount(value.Value);

    public CategoryOwnedVehiclesCount Increase()
    {
        long nextValue = Value + 1;
        return new CategoryOwnedVehiclesCount(nextValue);
    }
}
