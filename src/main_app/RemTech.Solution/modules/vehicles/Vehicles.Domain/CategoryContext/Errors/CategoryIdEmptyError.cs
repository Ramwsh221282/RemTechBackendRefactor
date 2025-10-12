using RemTech.Result.Pattern;

namespace Vehicles.Domain.CategoryContext.Errors;

public sealed record CategoryIdEmptyError()
    : Error("Идентификатор категории не может быть пустым.", ErrorCodes.Validation);
