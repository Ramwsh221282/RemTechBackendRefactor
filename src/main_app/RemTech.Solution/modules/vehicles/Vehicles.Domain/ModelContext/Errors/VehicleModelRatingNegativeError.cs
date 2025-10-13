using RemTech.Result.Pattern;

namespace Vehicles.Domain.ModelContext.Errors;

public sealed record VehicleModelRatingNegativeError()
    : Error("Рейтинг модели техники не может быть отрицательным.", ErrorCodes.Validation);
