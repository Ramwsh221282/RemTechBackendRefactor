using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Decorators;

public sealed class LoggingVehicleBrand(ICustomLogger logger, VehicleBrand brand)
    : VehicleBrand(brand.Identify())
{
    public VehicleBrand Log()
    {
        LogIdentity(Identify());
        return this;
    }

    private void LogIdentity(VehicleBrandIdentity identity)
    {
        logger.Info("Идентификационная информация бренда техники:");
        logger.Info("ID: {0}.", (Guid)identity.ReadId());
        logger.Info("Название: {0}.", (string)identity.ReadText());
    }
}
