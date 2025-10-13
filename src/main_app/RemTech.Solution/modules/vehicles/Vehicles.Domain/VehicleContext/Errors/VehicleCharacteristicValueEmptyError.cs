using RemTech.Result.Pattern;

namespace Vehicles.Domain.VehicleContext.Errors;

public sealed record VehicleCharacteristicValueEmptyError()
    : Error("Значение характеристики техники не может быть пустым.", ErrorCodes.Validation);
