using RemTech.Core.Shared.Exceptions;
using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators;

public sealed class LoggingVehicleKind(ICustomLogger logger, VehicleKind origin)
    : VehicleKind(origin)
{
    protected override VehicleKindIdentity Identity
    {
        get
        {
            try
            {
                logger.Info("Идентификационная информация типа техники:");
                VehicleKindIdentity identity = base.Identity;
                Guid id = identity.ReadId();
                string name = identity.ReadText();
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
