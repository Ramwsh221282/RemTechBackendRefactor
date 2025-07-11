using RemTech.Core.Shared.Functional;
using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Ports;

public sealed class LoggingCharacteristics(ICustomLogger logger, ICharacteristics origin)
    : ICharacteristics
{
    public Status<CharacteristicEnvelope> Add(string? name)
    {
        logger.Info("Добавление характеристики.");
        Status<CharacteristicEnvelope> adding = origin.Add(name);
        return adding.IsSuccess
            ? new LoggingCharacteristic(logger, adding.Value).Log()
            : new LoggingBadStatus<CharacteristicEnvelope>(logger, adding).Logged();
    }

    public MaybeBag<CharacteristicEnvelope> GetByName(string? name) =>
        new LoggingMaybeBag<CharacteristicEnvelope>(
            logger,
            origin.GetByName(name),
            "Получение характеристики по имени: {0}."
        ).Logged();
}
