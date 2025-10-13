using RemTech.Result.Pattern;

namespace Vehicles.Domain.LocationContext.Errors;

public sealed record LocationRatingEmptyError()
    : Error("Рейтинг локации не может быть пустым.", ErrorCodes.Validation);
