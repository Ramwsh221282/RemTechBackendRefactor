using RemTech.Result.Pattern;

namespace Vehicles.Domain.CategoryContext.Errors;

public sealed record CategoryNameExceesLengthError(int Length)
    : Error($"Название категории превышает длину: {Length}", ErrorCodes.Validation);
