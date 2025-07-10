using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators;

public sealed class LoggingGeoLocation(ICustomLogger logger, GeoLocationEnvelope location)
    : GeoLocationEnvelope(location.Identify())
{
    public GeoLocationEnvelope Log()
    {
        LogIdentity(Identify());
        return this;
    }

    private void LogIdentity(GeoLocationIdentity identity)
    {
        logger.Info("Идентификационные данные геолокации техники:");
        logger.Info("ID: {0}.", (Guid)identity.ReadId());
        logger.Info("Название: {0}.", (string)identity.ReadText());
    }
}
