namespace RemTech.ParsedAdvertisements.Core.Transport.Vehicles.ValueObjects;

public sealed record ParsedVehiclePhotos
{
    private readonly HashSet<ParsedVehiclePhoto> _photos;

    public ParsedVehiclePhotos(IEnumerable<ParsedVehiclePhoto> photos)
    {
        _photos = new HashSet<ParsedVehiclePhoto>(photos);
    }
}