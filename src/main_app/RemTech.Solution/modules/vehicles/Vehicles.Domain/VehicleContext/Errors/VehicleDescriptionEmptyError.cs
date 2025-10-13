using RemTech.Result.Pattern;

namespace Vehicles.Domain.VehicleContext.Errors;

public sealed record VehicleDescriptionEmptyError()
    : Error("Описание у техники не может быть пустым.", ErrorCodes.Validation);
