using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePhotos;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Photos;

public sealed class LoggingItemPhotos : IParsedVehiclePhotos
{
    private readonly Serilog.ILogger _log;
    private readonly IParsedVehiclePhotos _photos;

    public LoggingItemPhotos(Serilog.ILogger log, IParsedVehiclePhotos photos)
    {
        _log = log;
        _photos = photos;
    }

    public async Task<UniqueParsedVehiclePhotos> Read()
    {
        UniqueParsedVehiclePhotos photos = await _photos.Read();
        _log.Information("Photos amount: {0}.", photos.Amount());
        return photos;
    }
}
