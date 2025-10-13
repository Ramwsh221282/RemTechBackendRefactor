using RemTech.Result.Pattern;

namespace Vehicles.Domain.ModelContext.Errors;

public sealed record VehicleModelVehicleCountNegativeError()
    : Error("Количество техники у модели не может быть отрицательным.", ErrorCodes.Validation);
