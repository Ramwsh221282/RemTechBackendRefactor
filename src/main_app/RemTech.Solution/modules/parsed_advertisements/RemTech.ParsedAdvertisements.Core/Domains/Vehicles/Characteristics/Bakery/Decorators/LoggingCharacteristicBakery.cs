using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Bakery.Receipts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Bakery.Decorators;

public sealed class LoggingCharacteristicBakery : ICharacteristicBakery
{
    private readonly ICustomLogger _logger;
    private readonly ICharacteristicBakery _origin;

    public LoggingCharacteristicBakery(ICustomLogger logger, ICharacteristicBakery origin)
    {
        _logger = logger;
        _origin = origin;
    }

    public Status<ICharacteristic> Baked(ICharacteristicReceipt receipt)
    {
        _logger.Info("Создание характеристики транспорта.");
        Status<ICharacteristic> ctx = _origin.Baked(receipt);
        if (ctx.IsSuccess)
        {
            _logger.Info("Создана характеристика.");
            _logger.Info("ID: {0}.", (Guid)ctx.Value.Identify().ReadId());
            _logger.Info("Название: {0}.", (string)ctx.Value.Identify().ReadText());
            return ctx;
        }
        _logger.Error("Ошибка: {0}.", ctx.Error.ErrorText);
        return ctx;
    }
}
