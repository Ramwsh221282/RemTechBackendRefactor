using RemTech.Result.Pattern;

namespace Vehicles.Domain.BrandContext.Errors;

public sealed record BrandNegativeRatingError()
    : Error("Рейтинг бренда не может быть отрицательным.", ErrorCodes.Validation);
