using RemTech.Result.Pattern;

namespace Vehicles.Domain.BrandContext.Errors;

public sealed record BrandEmptyVehiclesCountError()
    : Error("Количество техники у бренда не может быть пустым.", ErrorCodes.Validation);
