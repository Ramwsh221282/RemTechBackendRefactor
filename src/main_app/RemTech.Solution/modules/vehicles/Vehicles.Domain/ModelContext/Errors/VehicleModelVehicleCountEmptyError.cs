using RemTech.Result.Pattern;

namespace Vehicles.Domain.ModelContext.Errors;

public sealed record VehicleModelVehicleCountEmptyError()
    : Error("Количество техники у модели не может быть пустым.", ErrorCodes.Validation);
