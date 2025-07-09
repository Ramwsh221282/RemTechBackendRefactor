using RemTech.Core.Shared.Primitives;
using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Identities;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Bakery;

public interface IVehicleReceipt
{
    Status<IVehicleKind> WhatKind();
    Status<IVehicleBrand> WhatBrand();
    Status<IGeoLocation> WhatGeoLocation();
    Dictionary<NotEmptyString, Status<ICharacteristic>> WhatCharacteristics();
    NotEmptyString WhatId();
    NotEmptyString[] PhotoSources();
    VehiclePrice WhatPrice();
    NotEmptyString WhatTitle();
    NotEmptyString WhatDescription();
}

public sealed class VehicleReceipt : IVehicleReceipt
{
    private readonly Status<IVehicleKind> _kind;
    private readonly Status<IVehicleBrand> _brand;
    private readonly Status<IGeoLocation> _geoLocation;
    private readonly Dictionary<NotEmptyString, Status<ICharacteristic>> _characteristics;
    private readonly NotEmptyString[] _photoSources;
    private readonly NotEmptyString _id;
    private readonly VehiclePrice _price;
    private readonly NotEmptyString _title;
    private readonly NotEmptyString _description;

    public VehicleReceipt(
        Status<IVehicleKind> kind,
        Status<IVehicleBrand> brand,
        Status<IGeoLocation> geoLocation,
        Dictionary<NotEmptyString, Status<ICharacteristic>> characteristics,
        NotEmptyString[] photoSources,
        NotEmptyString id,
        VehiclePrice price,
        NotEmptyString title,
        NotEmptyString description
    )
    {
        _kind = kind;
        _brand = brand;
        _geoLocation = geoLocation;
        _characteristics = characteristics;
        _photoSources = photoSources;
        _id = id;
        _price = price;
        _title = title;
        _description = description;
    }

    public Status<IVehicleKind> WhatKind() => _kind;

    public Status<IVehicleBrand> WhatBrand() => _brand;

    public Status<IGeoLocation> WhatGeoLocation() => _geoLocation;

    public Dictionary<NotEmptyString, Status<ICharacteristic>> WhatCharacteristics() =>
        _characteristics;

    public NotEmptyString WhatId() => _id;

    public NotEmptyString[] PhotoSources() => _photoSources;

    public VehiclePrice WhatPrice() => _price;

    public NotEmptyString WhatTitle()
    {
        return _title;
    }

    public NotEmptyString WhatDescription()
    {
        return _description;
    }
}

public interface IVehicleBakery
{
    public Status<IVehicle> Baked(IVehicleReceipt receipt);
}

public sealed class LoggingVehicleBakery : IVehicleBakery
{
    private readonly ICustomLogger _logger;
    private readonly IVehicleBakery _origin;

    public LoggingVehicleBakery(ICustomLogger logger, IVehicleBakery origin)
    {
        _logger = logger;
        _origin = origin;
    }

    public Status<IVehicle> Baked(IVehicleReceipt receipt)
    {
        _logger.Info("Создание техники.");
        Status<IVehicle> baked = _origin.Baked(receipt);
        if (baked.IsSuccess)
        {
            _logger.Info("Создана техника:");
            _logger.Info(
                "ID: {0}.",
                (string)
                    baked
                        .Value.Identify()
                        .WhatKind()
                        .WhatBrand()
                        .WhatLocation()
                        .WhatOrigin()
                        .ReadId()
            );
            _logger.Info("Заголовок: {0}.", (string)baked.Value.TextInformation().ReadTitle());
            _logger.Info(
                "Цена: {0}. НДС: {1}.",
                (long)baked.Value.WhatCost().Read().Value(),
                baked.Value.WhatCost().Read().UnderNds()
            );
            _logger.Info("Характеристики техники:");
            foreach (VehicleCharacteristic ctx in baked.Value.WhatCharacteristics().Read())
            {
                string name = ctx.WhatCharacteristics().ReadText();
                string value = ctx.WhatValue();
                _logger.Info("{0}, {1}.", name, value);
            }
            _logger.Info(
                "Геолокация ID: {0}.",
                (Guid)
                    baked
                        .Value.Identify()
                        .WhatKind()
                        .WhatBrand()
                        .WhatLocation()
                        .ReadLocation()
                        .ReadId()
            );
            _logger.Info(
                "Геолокация Название: {0}.",
                (string)
                    baked
                        .Value.Identify()
                        .WhatKind()
                        .WhatBrand()
                        .WhatLocation()
                        .ReadLocation()
                        .ReadText()
            );
            _logger.Info(
                "Бренд ID: {0}.",
                (Guid)baked.Value.Identify().WhatKind().WhatBrand().ReadBrand().ReadId()
            );
            _logger.Info(
                "Бренд Название: {0}.",
                (string)baked.Value.Identify().WhatKind().WhatBrand().ReadBrand().ReadText()
            );
            _logger.Info(
                "Тип ID: {0}.",
                (Guid)baked.Value.Identify().WhatKind().ReadKind().ReadId()
            );
            _logger.Info(
                "Тип Название: {0}.",
                (string)baked.Value.Identify().WhatKind().ReadKind().ReadText()
            );
            return baked;
        }
        _logger.Error("Ошибка: {0}.", baked.Error.ErrorText);
        return baked;
    }
}

public sealed class CachingVehicleBakery : IVehicleBakery
{
    private readonly IVehicleBakery _bakery;
    private MaybeBag<Status<IVehicle>> _bag;

    public CachingVehicleBakery(IVehicleBakery bakery)
    {
        _bakery = bakery;
        _bag = new MaybeBag<Status<IVehicle>>();
    }

    public Status<IVehicle> Baked(IVehicleReceipt receipt)
    {
        if (_bag.Any())
            return _bag.Take();
        _bag = _bag.Put(_bakery.Baked(receipt));
        return _bag.Take();
    }
}

public sealed class ValidatingVehicleBakery : IVehicleBakery
{
    private readonly IVehicleBakery _bakery;

    public ValidatingVehicleBakery(IVehicleBakery bakery)
    {
        _bakery = bakery;
    }

    public Status<IVehicle> Baked(IVehicleReceipt receipt)
    {
        if (receipt.WhatKind().IsFailure)
            return new ValidationError<IVehicle>(receipt.WhatKind().Error.ErrorText);
        if (receipt.WhatBrand().IsFailure)
            return new ValidationError<IVehicle>(receipt.WhatBrand().Error.ErrorText);
        if (receipt.WhatGeoLocation().IsFailure)
            return new ValidationError<IVehicle>(receipt.WhatGeoLocation().Error.ErrorText);
        if (receipt.WhatCharacteristics().Values.All(c => c.IsFailure))
            return new ValidationError<IVehicle>("Нельзя создать технику без характеристик.");
        if (!receipt.WhatId())
            return new ValidationError<IVehicle>("Нельзя создать технику без идентификатора.");
        if (!receipt.WhatDescription())
            return new ValidationError<IVehicle>("Нельзя создать технику без описания.");
        if (!receipt.WhatTitle())
            return new ValidationError<IVehicle>("Нельзя создать технику без заголовка.");
        if (receipt.PhotoSources().Length == 0)
            return new ValidationError<IVehicle>("Нельзя создать технику без фотографий..");
        return _bakery.Baked(receipt);
    }
}

public sealed class VehicleBakery : IVehicleBakery
{
    public Status<IVehicle> Baked(IVehicleReceipt receipt) =>
        new Vehicle(
            new VehicleIdentity(
                new VehicleIdentityOfKind(
                    receipt.WhatKind().Value.Identify(),
                    new BrandedVehicleIdentity(
                        receipt.WhatBrand().Value.Identify(),
                        new LocationedVehicleIdentity(
                            receipt.WhatGeoLocation().Value.Identify(),
                            new OriginVehicleIdentity(new VehicleId(receipt.WhatId()))
                        )
                    )
                )
            ),
            receipt.WhatPrice(),
            new VehicleText(receipt.WhatDescription(), receipt.WhatTitle()),
            new VehiclePhotos([.. receipt.PhotoSources().Select(s => new VehiclePhoto(s))]),
            new VehicleCharacteristics(
                [
                    .. receipt
                        .WhatCharacteristics()
                        .Where(c => c.Value.IsSuccess)
                        .Select(c => new VehicleCharacteristic(
                            c.Value.Value.Identify(),
                            new VehicleCharacteristicValue(c.Key)
                        )),
                ]
            )
        );
}
