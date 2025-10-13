using RemTech.Result.Pattern;

namespace Vehicles.Domain.ModelContext.Errors;

public sealed record VehicleModelNameExceesLengthError(int length)
    : Error(
        $"Название модели техники не может превышать длину {length} символов.",
        ErrorCodes.Validation
    );
