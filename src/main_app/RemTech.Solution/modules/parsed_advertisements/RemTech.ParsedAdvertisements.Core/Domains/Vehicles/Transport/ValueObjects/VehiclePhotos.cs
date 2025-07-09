namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;

public sealed record VehiclePhotos
{
    private readonly HashSet<VehiclePhoto> _photos;

    public VehiclePhotos(IEnumerable<VehiclePhoto> photos)
    {
        _photos = new HashSet<VehiclePhoto>(photos);
    }

    public VehiclePhotos()
    {
        _photos = [];
    }
}
