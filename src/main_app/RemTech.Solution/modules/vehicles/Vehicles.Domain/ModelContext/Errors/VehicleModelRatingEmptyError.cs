using RemTech.Result.Pattern;

namespace Vehicles.Domain.ModelContext.Errors;

public sealed record VehicleModelRatingEmptyError()
    : Error("Рейтинг модели техники не может быть пустым", ErrorCodes.Validation);
