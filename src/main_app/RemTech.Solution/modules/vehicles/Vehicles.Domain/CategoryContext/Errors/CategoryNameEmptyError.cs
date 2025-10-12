using RemTech.Result.Pattern;

namespace Vehicles.Domain.CategoryContext.Errors;

public sealed record CategoryNameEmptyError()
    : Error("Название категории техники не может быть пустым", ErrorCodes.Validation);
