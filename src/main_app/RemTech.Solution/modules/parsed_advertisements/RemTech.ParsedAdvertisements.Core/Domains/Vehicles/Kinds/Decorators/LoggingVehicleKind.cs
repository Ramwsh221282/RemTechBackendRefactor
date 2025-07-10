using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators;

public sealed class LoggingVehicleKind(ICustomLogger logger, VehicleKindEnvelope kind)
    : VehicleKindEnvelope(kind.Identify())
{
    public LoggingVehicleKind Log()
    {
        LogIdentity(Identify());
        return this;
    }

    private void LogIdentity(VehicleKindIdentity identity)
    {
        logger.Info("Идентификационная информация типа техники:");
        logger.Info("ID: {0}.", (Guid)identity.ReadId());
        logger.Info("Название: {0}.", (string)identity.ReadText());
    }
}
