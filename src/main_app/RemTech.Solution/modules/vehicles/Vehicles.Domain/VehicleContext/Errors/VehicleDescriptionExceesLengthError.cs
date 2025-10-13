using RemTech.Result.Pattern;

namespace Vehicles.Domain.VehicleContext.Errors;

public sealed record VehicleDescriptionExceesLengthError(int Length)
    : Error($"Длина описания техники превышает {Length} символов.", ErrorCodes.Validation);
