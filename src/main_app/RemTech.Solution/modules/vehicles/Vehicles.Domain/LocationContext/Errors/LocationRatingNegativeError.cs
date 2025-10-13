using RemTech.Result.Pattern;

namespace Vehicles.Domain.LocationContext.Errors;

public sealed record LocationRatingNegativeError()
    : Error("Рейтинг локации не может быть отрицательным.", ErrorCodes.Validation);
