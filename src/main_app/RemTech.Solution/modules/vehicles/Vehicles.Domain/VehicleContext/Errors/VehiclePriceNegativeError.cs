using RemTech.Result.Pattern;

namespace Vehicles.Domain.VehicleContext.Errors;

public sealed record VehiclePriceNegativeError()
    : Error("Цена техники не может быть отрицательной.", ErrorCodes.Validation);
