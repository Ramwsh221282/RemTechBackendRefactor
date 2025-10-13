using RemTech.Result.Pattern;

namespace Vehicles.Domain.ModelContext.Errors;

public sealed record VehicleModelNameEmptyError()
    : Error("Название модели техники не может быть пустым.", ErrorCodes.Validation);
