using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Bakery.Receipts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Bakery.Decorators;

public sealed class LoggingVehicleGeoBakery : IVehicleGeoBakery
{
    private readonly ICustomLogger _logger;
    private readonly IVehicleGeoBakery _bakery;

    public LoggingVehicleGeoBakery(ICustomLogger logger, IVehicleGeoBakery bakery)
    {
        _logger = logger;
        _bakery = bakery;
    }

    public Status<IGeoLocation> Baked(IVehicleGeoReceipt receipt)
    {
        _logger.Info("Создание геолокации.");
        Status<IGeoLocation> geo = _bakery.Baked(receipt);
        if (geo.IsSuccess)
        {
            _logger.Info("Геолокация создана:");
            _logger.Info("ID: {0}.", (Guid)geo.Value.Identify().ReadId());
            _logger.Info("Название: {0}.", (string)geo.Value.Identify().ReadText());
            return geo;
        }
        _logger.Error("Ошибка: {0}.", geo.Error.ErrorText);
        return geo;
    }
}
