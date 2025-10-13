using RemTech.Result.Pattern;

namespace Vehicles.Domain.LocationContext.Errors;

public sealed record LocationVehiclesCountEmptyError()
    : Error("Количество техники у локации не может быть пустым.", ErrorCodes.Validation);
