using RemTech.Result.Pattern;

namespace Vehicles.Domain.CategoryContext.Errors;

public sealed record CategoryOwnedVehiclesCountEmptyError()
    : Error("Количество транспорта у категории не может быть пустым.", ErrorCodes.Validation);
