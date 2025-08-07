using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePhotos;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Photos;

public sealed class DefaultOnErrorItemPhotos : IParsedVehiclePhotos
{
    private readonly IParsedVehiclePhotos _photos;

    public DefaultOnErrorItemPhotos(IParsedVehiclePhotos photos)
    {
        _photos = photos;
    }

    public async Task<UniqueParsedVehiclePhotos> Read()
    {
        try
        {
            return await _photos.Read();
        }
        catch
        {
            return new UniqueParsedVehiclePhotos();
        }
    }
}
