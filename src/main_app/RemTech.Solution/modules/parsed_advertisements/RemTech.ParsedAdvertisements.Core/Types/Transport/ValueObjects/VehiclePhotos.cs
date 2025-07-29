using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Types.Transport.ValueObjects;

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

    public PositiveInteger Amount()
    {
        return new PositiveInteger(_photos.Count);
    }

    public IEnumerable<VehiclePhoto> Read() => _photos;

    public static implicit operator bool(VehiclePhotos? photos)
    {
        return photos != null && photos.Amount() > 0 && photos._photos.All(p => p);
    }
}
