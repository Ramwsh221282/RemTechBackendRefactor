using RemTech.Result.Pattern;

namespace Vehicles.Domain.BrandContext.Errors;

public sealed record BrandNegativeVehiclesCountError()
    : Error("Количество техники у бренда не может быть отрицательным.", ErrorCodes.Validation);
