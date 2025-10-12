using RemTech.Result.Pattern;

namespace Vehicles.Domain.BrandContext.Errors;

public sealed record BrandRatingEmptyError()
    : Error("Рейтинг бренда не может быть пустым.", ErrorCodes.Validation);
