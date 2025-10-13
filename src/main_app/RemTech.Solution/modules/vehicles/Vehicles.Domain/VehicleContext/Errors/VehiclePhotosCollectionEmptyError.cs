using RemTech.Result.Pattern;

namespace Vehicles.Domain.VehicleContext.Errors;

public sealed record VehiclePhotosCollectionEmptyError()
    : Error("Список фотографий техники пуст.", ErrorCodes.Validation);
