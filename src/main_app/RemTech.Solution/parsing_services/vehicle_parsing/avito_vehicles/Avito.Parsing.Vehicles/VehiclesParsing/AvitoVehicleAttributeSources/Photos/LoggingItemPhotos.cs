using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePhotos;
using RemTech.Logging.Library;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Photos;

public sealed class LoggingItemPhotos : IParsedVehiclePhotos
{
    private readonly ICustomLogger _log;
    private readonly IParsedVehiclePhotos _photos;

    public LoggingItemPhotos(ICustomLogger log, IParsedVehiclePhotos photos)
    {
        _log = log;
        _photos = photos;
    }
    
    public async Task<UniqueParsedVehiclePhotos> Read()
    {
        UniqueParsedVehiclePhotos photos = await _photos.Read();
        _log.Info("Photos amount: {0}.", photos.Amount());
        return photos;
    }
}