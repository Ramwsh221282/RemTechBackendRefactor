using RemTech.Result.Pattern;

namespace Vehicles.Domain.CategoryContext.Errors;

public sealed record CategoryRatingNegativeError()
    : Error("Рейтинг категории не может быть отрицательным.", ErrorCodes.Validation);
