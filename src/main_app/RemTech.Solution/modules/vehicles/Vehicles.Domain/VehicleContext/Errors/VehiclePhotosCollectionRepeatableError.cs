using RemTech.Result.Pattern;
using Vehicles.Domain.VehicleContext.ValueObjects;

namespace Vehicles.Domain.VehicleContext.Errors;

public sealed record VehiclePhotosCollectionRepeatableError : Error
{
    public VehiclePhotosCollectionRepeatableError(IEnumerable<VehiclePhoto> photos)
        : this(photos.Select(p => p.Path)) { }

    public VehiclePhotosCollectionRepeatableError(IEnumerable<string> paths)
        : base(
            $"Список фотографий должен состоять из уникальных частей. Повторяющиеся части: {string.Join(" ,", paths)}",
            ErrorCodes.Validation
        ) { }
}
