using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Bakery.Receipts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Bakery.Decorators;

public sealed class LoggingVehicleKindBakery : IVehicleKindBakery
{
    private readonly ICustomLogger _logger;
    private readonly IVehicleKindBakery _origin;

    public LoggingVehicleKindBakery(ICustomLogger logger, IVehicleKindBakery origin)
    {
        _logger = logger;
        _origin = origin;
    }

    public Status<IVehicleKind> Baked(IVehicleKindReceipt receipt)
    {
        _logger.Info("Создание типа техники.");
        Status<IVehicleKind> kind = _origin.Baked(receipt);
        if (kind.IsSuccess)
        {
            _logger.Info("Тип техники создан.");
            _logger.Info("ID: {0}.", kind.Value.Identify().ReadId());
            _logger.Info("Название: {0}.", kind.Value.Identify().ReadText());
            return kind;
        }
        _logger.Info("Ошибка: {0}.", kind.Error.ErrorText);
        return kind;
    }
}
