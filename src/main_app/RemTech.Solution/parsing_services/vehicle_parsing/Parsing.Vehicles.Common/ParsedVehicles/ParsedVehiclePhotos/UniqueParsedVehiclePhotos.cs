namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePhotos;

public sealed  class UniqueParsedVehiclePhotos
{
    private readonly HashSet<string> _photos;

    public UniqueParsedVehiclePhotos(params string[] photos) =>
        _photos = new HashSet<string>(photos);

    public UniqueParsedVehiclePhotos(IEnumerable<string> photos) =>
        _photos = new HashSet<string>(photos);

    public UniqueParsedVehiclePhotos With(string photo)
    {
        _photos.Add(photo);
        return this;
    }
    
    public int Amount() => _photos.Count;
    
    public string[] Read() => _photos.ToArray();
}