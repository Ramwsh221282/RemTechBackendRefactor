using RemTech.Result.Pattern;

namespace Vehicles.Domain.VehicleContext.Errors;

public sealed record VehicleCharacteristicValueExceesLengthError(int Length)
    : Error(
        $"Значение характеристики техники превышает длину {Length} символов.",
        ErrorCodes.Validation
    );
