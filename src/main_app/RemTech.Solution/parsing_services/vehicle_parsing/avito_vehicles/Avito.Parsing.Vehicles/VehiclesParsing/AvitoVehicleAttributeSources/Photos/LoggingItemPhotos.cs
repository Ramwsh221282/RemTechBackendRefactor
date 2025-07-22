using Parsing.SDK.Logging;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePhotos;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Photos;

public sealed class LoggingItemPhotos : IParsedVehiclePhotos
{
    private readonly IParsingLog _log;
    private readonly IParsedVehiclePhotos _photos;

    public LoggingItemPhotos(IParsingLog log, IParsedVehiclePhotos photos)
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