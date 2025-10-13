using RemTech.Result.Pattern;

namespace Vehicles.Domain.LocationContext.Errors;

public sealed record LocationAddressPartEmptyError()
    : Error("Часть локации была пустой.", ErrorCodes.Validation);
