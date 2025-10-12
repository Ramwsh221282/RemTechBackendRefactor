using RemTech.Result.Pattern;

namespace Vehicles.Domain.BrandContext.Errors;

public sealed record BrandNameEmptyError()
    : Error("Название бренда не может быть пустым.", ErrorCodes.Validation);
