using RemTech.Result.Pattern;

namespace Vehicles.Domain.VehicleContext.Errors;

public sealed record VehicleIdEmptyError()
    : Error("ИД техники не может быть пустым.", ErrorCodes.Validation);
