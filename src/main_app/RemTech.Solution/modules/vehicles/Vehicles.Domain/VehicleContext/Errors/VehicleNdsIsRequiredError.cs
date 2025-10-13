using RemTech.Result.Pattern;

namespace Vehicles.Domain.VehicleContext.Errors;

public sealed record VehicleNdsIsRequiredError()
    : Error("НДС цены должен быть указан.", ErrorCodes.Validation);
