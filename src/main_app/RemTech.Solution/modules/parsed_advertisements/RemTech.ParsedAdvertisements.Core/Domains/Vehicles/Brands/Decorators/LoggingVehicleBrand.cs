using RemTech.Core.Shared.Exceptions;
using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Adapters.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Decorators;

public sealed class LoggingVehicleBrand(ICustomLogger logger, VehicleBrand brand)
    : VehicleBrand(brand)
{
    public override PgVehicleBrandFromStoreCommand FromStoreCommand()
    {
        try
        {
            VehicleBrandIdentity identity = base.Identify();
            string name = identity.ReadText();
            Guid id = identity.ReadId();
            logger.Info("Бренд ID - {0}. Название - {1} создат команду получения из хранилища.", id, name);
            return base.FromStoreCommand();
        }
        catch(Exception ex) when (ex is ValueNotValidException validation)
        {
            logger.Error("Ошибка: {0}.", validation.Message);
            throw;
        }
    }

    public override VehicleBrandIdentity Identify()
    {
        try
        {
            logger.Info("Идентификационные данные бренда:");
            VehicleBrandIdentity identity = base.Identify();
            string name = identity.ReadText();
            Guid id = identity.ReadId();
            logger.Info("ID - {0}. Название - {1}.", name, id);
            return identity;
        }
        catch(Exception ex) when (ex is ValueNotValidException validation)
        {
            logger.Error("Ошибка: {0}.", validation.Message);
            throw;
        }
    }

    public override PgVehicleBrandStoreCommand StoreCommand()
    {
        try
        {
            VehicleBrandIdentity identity = base.Identify();
            string name = identity.ReadText();
            Guid id = identity.ReadId();
            logger.Info("Бренд ID - {0}. Название - {1} создат команду сохранение в хранилище.", id, name);
            return base.StoreCommand();
        }
        catch(Exception ex) when (ex is ValueNotValidException validation)
        {
            logger.Error("Ошибка: {0}.", validation.Message);
            throw;
        }
    }

    public override BrandedVehicleModel Print(BrandedVehicleModel branded)
    {
        try
        {
            logger.Info("Бренд брендует модель техники");
            return base.Print(branded);
        }
        catch(Exception ex) when (ex is ValueNotValidException validation)
        {
            logger.Error("Ошибка: {0}.", validation.Message);
            throw;
        }
    }

    public override Vehicle Print(Vehicle vehicle)
    {
        try
        {
            logger.Info("Бренд брендует технику");
            return base.Print(vehicle);
        }
        catch(Exception ex) when (ex is ValueNotValidException validation)
        {
            logger.Error("Ошибка: {0}.", validation.Message);
            throw;
        }
    }
}
