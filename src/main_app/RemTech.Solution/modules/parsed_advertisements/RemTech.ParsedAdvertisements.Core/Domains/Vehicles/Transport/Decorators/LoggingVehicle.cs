using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;

public sealed class LoggingVehicle(ICustomLogger logger, VehicleEnvelope origin)
    : VehicleEnvelope(origin)
{
    public LoggingVehicle Log()
    {
        LogIdentity(Identity());
        LogKind(Kind().Identify());
        LogBrand(Brand().Identify());
        LogGeo(Location().Identify());
        LogText(TextInformation());
        LogPrice(Cost());
        LogPhotoCount(Photos());
        LogCharacteristics(Characteristics());
        return this;
    }

    private void LogIdentity(VehicleIdentity identity)
    {
        logger.Info("Идентификационные данные техники:");
        logger.Info("ID: {0}.", (string)identity.Read());
    }

    private void LogKind(VehicleKindIdentity kind)
    {
        logger.Info("Информация о типе техники:");
        logger.Info("ID типа техники: {0}.", (Guid)kind.ReadId());
        logger.Info("Название типа техники: {0}.", (string)kind.ReadText());
    }

    private void LogBrand(VehicleBrandIdentity brand)
    {
        logger.Info("Информация о бренде техники:");
        logger.Info("ID бренда: {0}.", (Guid)brand.ReadId());
        logger.Info("Название бренда техники: {0}.", (string)brand.ReadText());
    }

    private void LogGeo(GeoLocationIdentity location)
    {
        logger.Info("Информация о геолокации техники:");
        logger.Info("ID геолокации: {0}.", (Guid)location.ReadId());
        logger.Info("Название геолокации: {0}.", (string)location.ReadText());
    }

    private void LogText(VehicleText text)
    {
        logger.Info("Текстовая информация техники:");
        logger.Info("Заголовок: {0}.", (string)text.ReadTitle());
        logger.Info("Описание: {0}.", (string)text.ReadDescription());
    }

    private void LogPrice(IItemPrice price)
    {
        logger.Info("Информация о цене техники:");
        logger.Info("Стоимость: {0}.", (long)price.Value());
        logger.Info("Цена с НДС?: {0}.", price.UnderNds());
    }

    private void LogPhotoCount(VehiclePhotos photos)
    {
        logger.Info("Количество фотографий: {0}.", (int)photos.Amount());
    }

    private void LogCharacteristics(VehicleCharacteristics characteristics)
    {
        logger.Info("Характеристики техники:");
        logger.Info("Количество: {0}.", (int)characteristics.Amount());
        foreach (VehicleCharacteristic ctx in characteristics.Read())
            logger.Info(
                "{0} - {1}.",
                (string)ctx.WhatCharacteristic().Identify().ReadText(),
                (string)ctx.WhatValue()
            );
    }
}
