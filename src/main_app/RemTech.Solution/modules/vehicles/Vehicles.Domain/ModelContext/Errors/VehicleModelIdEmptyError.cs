using RemTech.Result.Pattern;

namespace Vehicles.Domain.ModelContext.Errors;

public sealed record VehicleModelIdEmptyError()
    : Error("Идентификатор модели техники не может быть пустым.", ErrorCodes.Validation);
