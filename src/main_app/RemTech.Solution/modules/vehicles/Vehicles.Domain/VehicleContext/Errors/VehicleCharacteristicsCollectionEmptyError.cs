using RemTech.Result.Pattern;

namespace Vehicles.Domain.VehicleContext.Errors;

public sealed record VehicleCharacteristicsCollectionEmptyError()
    : Error("Список характеристик техники был пустым", ErrorCodes.Validation);
