using RemTech.Result.Pattern;

namespace Vehicles.Domain.VehicleContext.Errors;

public sealed record VehiclePhotoPathEmptyError()
    : Error("Путь к фотографии техники не может быть пустым.", ErrorCodes.Validation);
