using RemTech.Result.Pattern;

namespace Vehicles.Domain.BrandContext.Errors;

public sealed record BrandNameExceesLengthError(int Length)
    : Error($"Название бренда превышает длину {Length} символов.", ErrorCodes.Validation);
