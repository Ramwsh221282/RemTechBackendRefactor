using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Ports;

public sealed class ValidatingVehicles(IVehicles origin) : IVehicles
{
    public Status<VehicleEnvelope> Add(VehicleEnvelope vehicle)
    {
        VehicleBrandIdentity brand = vehicle.Brand().Identify();
        if (!brand.ReadText())
            return new ValidationError<VehicleEnvelope>(
                $"Некорректное название бренда техники: {(string)brand.ReadText()}"
            );
        if (!brand.ReadId())
            return new ValidationError<VehicleEnvelope>(
                $"Некорректное ID бренда техники: {(Guid)brand.ReadId()}"
            );
        VehicleKindIdentity kind = vehicle.Kind().Identify();
        if (!kind.ReadText())
            return new ValidationError<VehicleEnvelope>(
                $"Некорректное название типа техники: {(string)kind.ReadText()}"
            );
        if (!kind.ReadId())
            return new ValidationError<VehicleEnvelope>(
                $"Некорректное ID типа техники: {(Guid)kind.ReadId()}"
            );
        GeoLocationIdentity geo = vehicle.Location().Identify();
        if (!geo.ReadText())
            return new ValidationError<VehicleEnvelope>(
                $"Некорректное название геолокации техники: {(string)geo.ReadText()}"
            );
        if (!geo.ReadId())
            return new ValidationError<VehicleEnvelope>(
                $"Некорректное ID геолокации техники: {(string)geo.ReadText()}"
            );
        NotEmptyString title = vehicle.TextInformation().ReadTitle();
        if (!title)
            return new ValidationError<VehicleEnvelope>(
                $"Некорректный заголовок техники: {(string)title}"
            );
        NotEmptyString description = vehicle.TextInformation().ReadDescription();
        if (!description)
            return new ValidationError<VehicleEnvelope>(
                $"Некорректное описание техники: {(string)title}"
            );
        VehicleId identity = vehicle.Identity().Read();
        if (!identity)
            return new ValidationError<VehicleEnvelope>(
                $"Некорректное ID техники: {(string)identity}"
            );
        if (vehicle.Characteristics().Amount() == 0)
            return new ValidationError<VehicleEnvelope>(
                "У техники не может не быть характеристик."
            );
        VehicleCharacteristic[] ctx = vehicle.Characteristics().Read();
        foreach (VehicleCharacteristic entry in ctx)
        {
            CharacteristicIdentity ctxIdentity = entry.WhatCharacteristic().Identify();
            if (!ctxIdentity.ReadText())
                return new ValidationError<VehicleEnvelope>(
                    $"Некорректное название характеристики: {(string)ctxIdentity.ReadText()}"
                );
            if (!ctxIdentity.ReadId())
                return new ValidationError<VehicleEnvelope>(
                    $"Некорректный ID характеристики: {(Guid)ctxIdentity.ReadId()}"
                );
            VehicleCharacteristicValue value = entry.WhatValue();
            if (!value)
                return new ValidationError<VehicleEnvelope>(
                    $"Некорректное значение характеристики: {(string)value}"
                );
        }

        if (vehicle.Photos().Amount() == 0)
            return new ValidationError<VehicleEnvelope>("У техники не может не быть фотографий");
        VehiclePhotos photos = vehicle.Photos();
        foreach (var photo in photos.Read())
        {
            if (!photo)
                return new ValidationError<VehicleEnvelope>(
                    $"Некорректный источник фото техники: {(string)photo}"
                );
        }
        return origin.Add(vehicle);
    }

    public MaybeBag<VehicleEnvelope> Get(Func<VehicleEnvelope, bool> predicate)
    {
        return origin.Get(predicate);
    }
}
