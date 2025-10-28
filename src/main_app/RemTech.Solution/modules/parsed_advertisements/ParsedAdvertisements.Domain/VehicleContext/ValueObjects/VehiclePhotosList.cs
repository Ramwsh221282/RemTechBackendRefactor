namespace ParsedAdvertisements.Domain.VehicleContext.ValueObjects;

public sealed record VehiclePhotosList(IReadOnlyList<VehiclePhoto> Photos)
{
    public VehiclePhotosList(UniquePhotoColleciton photos) : this(photos.ToVehiclePhotosList())
    {
    }
}