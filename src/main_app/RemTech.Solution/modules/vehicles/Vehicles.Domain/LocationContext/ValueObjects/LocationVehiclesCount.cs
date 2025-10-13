using RemTech.Result.Pattern;
using Vehicles.Domain.LocationContext.Errors;

namespace Vehicles.Domain.LocationContext.ValueObjects;

public readonly record struct LocationVehiclesCount
{
    public long Value { get; }

    public LocationVehiclesCount() => Value = 0;

    private LocationVehiclesCount(long value) => Value = value;

    public static Result<LocationVehiclesCount> Create(long value) =>
        value < 0 ? new LocationVehiclesCountEmptyError() : new LocationVehiclesCount(value);

    public static Result<LocationVehiclesCount> Create(long? value) =>
        value == null
            ? new LocationVehiclesNegativeError()
            : new LocationVehiclesCount(value.Value);
}
