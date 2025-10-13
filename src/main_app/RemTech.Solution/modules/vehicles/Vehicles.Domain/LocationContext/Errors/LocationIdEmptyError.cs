using RemTech.Result.Pattern;

namespace Vehicles.Domain.LocationContext.Errors;

public sealed record LocationIdEmptyError()
    : Error("Идентификатор локации был пустым.", ErrorCodes.Validation);
