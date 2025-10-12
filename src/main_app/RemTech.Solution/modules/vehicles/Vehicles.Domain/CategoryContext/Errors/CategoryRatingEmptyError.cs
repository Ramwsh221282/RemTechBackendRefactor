using RemTech.Result.Pattern;

namespace Vehicles.Domain.CategoryContext.Errors;

public sealed record CategoryRatingEmptyError()
    : Error("Рейтинг категории не может быть пустым.", ErrorCodes.Validation);
