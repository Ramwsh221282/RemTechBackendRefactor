using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Decorators;

public sealed class LoggingCharacteristic(
    ICustomLogger logger,
    CharacteristicEnvelope characteristic
) : CharacteristicEnvelope(characteristic.Identify())
{
    public CharacteristicEnvelope Log()
    {
        LogIdentity(Identify());
        return this;
    }

    private void LogIdentity(CharacteristicIdentity identity)
    {
        logger.Info("Идентификационная информация характеристики техники:");
        logger.Info("ID: {0}.", (Guid)identity.ReadId());
        logger.Info("Название: {0}.", (string)identity.ReadText());
    }
}
