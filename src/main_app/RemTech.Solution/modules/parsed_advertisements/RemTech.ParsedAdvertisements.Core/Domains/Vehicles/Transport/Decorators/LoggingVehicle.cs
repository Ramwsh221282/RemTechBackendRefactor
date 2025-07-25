using RemTech.Core.Shared.Exceptions;
using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;

public sealed class LoggingVehicle(ICustomLogger logger, Vehicle vehicle) : Vehicle(vehicle)
{
    protected override VehicleIdentity Identity
    {
        get
        {
            try
            {
                VehicleIdentity identity = base.Identity;
                string id = identity.Read();
                logger.Info("Идентификатор техники: {0}.", id);
                return identity;
            }
            catch(Exception ex) when (ex is ValueNotValidException validation)
            {
                logger.Error("Ошибка: {0}.", validation.Message);
                throw;
            }
        }
    }

    protected override IVehicleBrand Brand
    {
        get
        {
            try
            {
                IVehicleBrand brand = base.Brand;
                string name = brand.Identify().ReadText();
                Guid id = brand.Identify().ReadId();
                logger.Info("Бренд техники: ID: - {0}. Название - {1}.", id, name);
                return brand;
            }
            catch(Exception ex) when (ex is ValueNotValidException validation)
            {
                logger.Error("Ошибка: {0}.", validation.Message);
                throw;
            }
        }
    }

    protected override VehicleCharacteristics Characteristics
    {
        get
        {
            try
            {
                VehicleCharacteristics characteristics = base.Characteristics;
                logger.Info("Характеристики техники:");
                int amount = characteristics.Amount();
                logger.Info("Количество характеристик: {0}.", amount);
                foreach (VehicleCharacteristic ctx in characteristics.Read())
                {
                    string name = ctx.WhatCharacteristic().Identify().ReadText();
                    string measure = ctx.WhatCharacteristic().Measure().Read();
                    string value = ctx.WhatValue();
                    logger.Info("Характеристика: {0} - {1} - {2}.", name, value, measure);
                }
                return characteristics;
            }
            catch(Exception ex) when (ex is ValueNotValidException validation)
            {
                logger.Error("Ошибка: {0}.", validation.Message);
                throw;
            }
        }
    }

    protected override IVehicleKind Kind
    {
        get
        {
            try
            {
                IVehicleKind kind = base.Kind;
                string name = kind.Identify().ReadText();
                Guid id = kind.Identify().ReadId();
                logger.Info("Тип техники: ID - {0}. Название - {1}.", id, name);
                return kind;
            }
            catch(Exception ex) when (ex is ValueNotValidException validation)
            {
                logger.Error("Ошибка: {0}.", validation.Message);
                throw;
            }
        }
    }

    protected override IGeoLocation Location
    {
        get
        {
            try
            {
                IGeoLocation location = base.Location;
                string name = location.Identify().ReadText();
                Guid id = location.Identify().ReadId();
                logger.Info("Локация техники: ID - {0}. Название - {1}.", id, name);
                return location;
            }
            catch(Exception ex) when (ex is ValueNotValidException validation)
            {
                logger.Error("Ошибка: {0}.", validation.Message);
                throw;
            }
        }
    }

    protected override IItemPrice Price
    {
        get
        {
            try
            {
                IItemPrice price = base.Price;
                long value = price.Value();
                bool isNds = price.UnderNds();
                logger.Info("Цена техники: {0} - {1}.", value, isNds);
                return price;
            }
            catch(Exception ex) when (ex is ValueNotValidException validation)
            {
                logger.Error("Ошибка: {0}.", validation.Message);
                throw;
            }
        }
    }

    protected override VehiclePhotos Photos
    {
        get
        {
            try
            {
                VehiclePhotos photos = base.Photos;
                int amount = photos.Amount();
                logger.Info("Количество фотографий техники - {0}.", amount);
                return photos;
            }
            catch(Exception ex) when (ex is ValueNotValidException validation)
            {
                logger.Error("Ошибка: {0}.", validation.Message);
                throw;
            }
        }
    }
}