using RemTech.Core.Shared.Exceptions;
using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators;

public sealed class LoggingGeoLocation(ICustomLogger logger, GeoLocation location)
    : GeoLocation(location)
{
    protected override GeoLocationIdentity Identity
    {
        get
        {
            try
            {
                GeoLocationIdentity identity = base.Identity;
                Guid id = identity.ReadId();
                string name = identity.ReadText();
                logger.Info("Идентификационные данные геолокации техники:");
                logger.Info("ID - {0}. Название - {1}.", id, name);
                return identity;
            }
            catch(Exception ex) when (ex is ValueNotValidException validation)
            {
                logger.Error("Ошибка: {0}.", validation.Message);
                throw;
            }
        }
    }
}
