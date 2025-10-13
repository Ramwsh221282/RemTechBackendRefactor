using RemTech.Result.Pattern;

namespace Vehicles.Domain.VehicleContext.Errors;

public sealed record VehicleCharacteristicNameEmptyError()
    : Error("Название характеристики техники не может быть пустым.", ErrorCodes.Validation);
