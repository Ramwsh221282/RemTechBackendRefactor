using RemTech.Result.Pattern;
using Vehicles.Domain.ModelContext.Errors;

namespace Vehicles.Domain.ModelContext.ValueObjects;

public readonly record struct VehicleModelVehicleCount
{
    public long Value { get; }

    public VehicleModelVehicleCount() => Value = 0;

    private VehicleModelVehicleCount(long value) => Value = value;

    public VehicleModelVehicleCount Increase()
    {
        long nextValue = Value + 1;
        return new VehicleModelVehicleCount(nextValue);
    }

    public static Result<VehicleModelVehicleCount> Create(long value) =>
        value < 0
            ? new VehicleModelVehicleCountNegativeError()
            : new VehicleModelVehicleCount(value);

    public static Result<VehicleModelVehicleCount> Create(long? value) =>
        value == null ? new VehicleModelVehicleCountEmptyError() : Create(value.Value);
}
