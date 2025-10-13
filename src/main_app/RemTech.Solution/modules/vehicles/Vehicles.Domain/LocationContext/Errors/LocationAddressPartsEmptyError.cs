using RemTech.Result.Pattern;

namespace Vehicles.Domain.LocationContext.Errors;

public sealed record LocationAddressPartsEmptyError()
    : Error("Адрес не может быть пустым.", ErrorCodes.Validation);
