using RemTech.Result.Pattern;

namespace Vehicles.Domain.CategoryContext.Errors;

public sealed record CategoryOwnedVehiclesCountNegativeError()
    : Error(
        "Количество транспорта у категории не может быть отрицательным.",
        ErrorCodes.Validation
    );
