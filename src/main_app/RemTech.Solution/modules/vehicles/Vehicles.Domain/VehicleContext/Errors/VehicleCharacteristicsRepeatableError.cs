using RemTech.Result.Pattern;
using Vehicles.Domain.VehicleContext.ValueObjects;

namespace Vehicles.Domain.VehicleContext.Errors;

public sealed record VehicleCharacteristicsRepeatableError : Error
{
    public VehicleCharacteristicsRepeatableError(IEnumerable<VehicleCharacteristic> ctx)
        : this(ctx.Select(c => c.Name)) { }

    public VehicleCharacteristicsRepeatableError(IEnumerable<string> repeatable)
        : base(
            $"Характеристики техники: {string.Join(" ,", repeatable)} повторяются.",
            ErrorCodes.Validation
        ) { }
}
