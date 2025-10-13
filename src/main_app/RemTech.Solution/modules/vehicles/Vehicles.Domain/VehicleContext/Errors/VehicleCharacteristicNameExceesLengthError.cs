using RemTech.Result.Pattern;

namespace Vehicles.Domain.VehicleContext.Errors;

public sealed record VehicleCharacteristicNameExceesLengthError(int Length)
    : Error(
        $"Название характеристики техники превышает длину {Length} символов.",
        ErrorCodes.Validation
    );
