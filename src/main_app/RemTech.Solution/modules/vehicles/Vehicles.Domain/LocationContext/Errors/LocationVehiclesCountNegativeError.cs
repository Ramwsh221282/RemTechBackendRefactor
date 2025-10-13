using RemTech.Result.Pattern;

namespace Vehicles.Domain.LocationContext.Errors;

public sealed record LocationVehiclesCountNegativeError()
    : Error("Количество техники у локации не может быть отрицательным.", ErrorCodes.Validation);
