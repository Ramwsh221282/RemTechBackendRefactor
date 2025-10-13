using RemTech.Result.Pattern;

namespace Vehicles.Domain.VehicleContext.Errors;

public sealed record VehiclePriceEmptyError()
    : Error("Цена техники не может пустой.", ErrorCodes.Validation);
