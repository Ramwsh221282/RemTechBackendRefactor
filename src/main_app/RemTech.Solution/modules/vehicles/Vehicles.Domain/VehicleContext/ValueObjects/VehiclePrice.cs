using RemTech.Result.Pattern;
using Vehicles.Domain.VehicleContext.Errors;

namespace Vehicles.Domain.VehicleContext.ValueObjects;

public readonly record struct VehiclePrice
{
    public long Value { get; }
    public bool IsNds { get; }

    public VehiclePrice()
    {
        Value = 0;
        IsNds = false;
    }

    private VehiclePrice(long value, bool isNds)
    {
        Value = value;
        IsNds = isNds;
    }

    public static Result<VehiclePrice> Create(long value, bool isNds) =>
        value < 0 ? new VehiclePriceNegativeError() : new VehiclePrice(value, isNds);

    public static Result<VehiclePrice> Create(long? value, bool? isNds)
    {
        if (value == null)
            return new VehiclePriceEmptyError();
        if (isNds == null)
            return new VehicleNdsIsRequiredError();
        return new VehiclePrice(value.Value, isNds.Value);
    }
}
