using RemTech.Result.Pattern;

namespace Vehicles.Domain.BrandContext.Errors;

public sealed record BrandIdEmptyError()
    : Error("Идентификатор бренда был пустым.", ErrorCodes.Validation);
