using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.VehicleContext.ValueObjects;

public sealed class VehiclePhotosList
{
    private readonly HashSet<VehiclePhoto> _photos;

    private VehiclePhotosList(IEnumerable<VehiclePhoto> photos) => _photos = [.. photos];

    public VehiclePhotosList() => _photos = [];

    public static Status<VehiclePhotosList> Create(IEnumerable<VehiclePhoto> photos)
    {
        var array = photos.ToArray();
        var distinct = photos.DistinctBy(p => p.Path).ToArray();
        return array.Length != distinct.Length
            ? Error.Validation("Фотографии техники должны быть уникальными")
            : new VehiclePhotosList(photos);
    }
}