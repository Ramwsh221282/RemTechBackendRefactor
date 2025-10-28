using RemTech.Core.Shared.Enumerable;
using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.VehicleContext.ValueObjects;

public sealed class UniquePhotoColleciton
{
    private readonly IEnumerable<VehiclePhoto> _photos;

    private UniquePhotoColleciton(IEnumerable<VehiclePhoto> photos)
    {
        _photos = [..photos];
    }

    public static Status<UniquePhotoColleciton> Create(IEnumerable<string> photos)
    {
        var photoResults = photos.Select(VehiclePhoto.Create).ToArray();
        var collection = new StatusCollection<VehiclePhoto>(photoResults);
        if (!collection.AllValid(out var error, out var value))
            return error;
        return Create(value);
    }

    public static Status<UniquePhotoColleciton> Create(IEnumerable<VehiclePhoto> photos)
    {
        var array = photos.ToArray();
        if (!array.AllUnique(el => el.Path))
            return Error.Validation("Список фотографий объявления должен быть уникален.");
        return new UniquePhotoColleciton(array);
    }

    public VehiclePhotosList ToVehiclePhotosList()
    {
        return new VehiclePhotosList(_photos.ToList());
    }
}