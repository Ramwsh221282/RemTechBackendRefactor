using RemTech.Result.Pattern;
using Vehicles.Domain.ModelContext.ValueObjects;

namespace Vehicles.Domain.ModelContext.Errors;

public sealed record NotUniqueVehicleModelError(VehicleModelName name)
    : Error($"Модель с названием {name.Value} уже существует.", ErrorCodes.Conflict);
