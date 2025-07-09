using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Bakery.Receipts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Bakery.Decorators;

public sealed class LoggingVehicleBrandBakery : IVehicleBrandBakery
{
    private readonly ICustomLogger _logger;
    private readonly IVehicleBrandBakery _bakery;

    public LoggingVehicleBrandBakery(ICustomLogger logger, IVehicleBrandBakery bakery)
    {
        _logger = logger;
        _bakery = bakery;
    }

    public Status<IVehicleBrand> Bake(IVehicleBrandReceipt receipt)
    {
        Status<IVehicleBrand> baked = _bakery.Bake(receipt);
        if (baked.IsSuccess)
        {
            _logger.Info("Создан бренд:");
            _logger.Info("ID: {0}.", (Guid)baked.Value.Identify().ReadId());
            _logger.Info("Название: {0}.", (string)baked.Value.Identify().ReadText());
            return baked;
        }
        _logger.Info("Ошибка создании бренда: {0}.", baked.Error.ErrorText);
        return baked;
    }
}
