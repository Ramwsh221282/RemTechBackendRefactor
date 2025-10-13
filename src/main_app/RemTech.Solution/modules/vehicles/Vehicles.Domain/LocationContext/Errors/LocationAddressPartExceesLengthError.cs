using RemTech.Result.Pattern;

namespace Vehicles.Domain.LocationContext.Errors;

public sealed record LocationAddressPartExceesLengthError(int Length)
    : Error($"Часть локации превышает длину {Length} символов.", ErrorCodes.Validation);
