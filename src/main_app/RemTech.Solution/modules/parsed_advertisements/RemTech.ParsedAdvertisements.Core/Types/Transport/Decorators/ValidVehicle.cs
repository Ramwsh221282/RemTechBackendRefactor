using RemTech.Core.Shared.Exceptions;
using RemTech.ParsedAdvertisements.Core.Types.Brands;
using RemTech.ParsedAdvertisements.Core.Types.Brands.Decorators.Logic;
using RemTech.ParsedAdvertisements.Core.Types.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Types.GeoLocations.Decorators.Logic;
using RemTech.ParsedAdvertisements.Core.Types.Kinds;
using RemTech.ParsedAdvertisements.Core.Types.Kinds.Decorators.Logic;
using RemTech.ParsedAdvertisements.Core.Types.Transport.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Types.Transport.ValueObjects.Characteristics;
using RemTech.ParsedAdvertisements.Core.Types.Transport.ValueObjects.Prices;

namespace RemTech.ParsedAdvertisements.Core.Types.Transport.Decorators;

public sealed class ValidVehicle(Vehicle origin) : Vehicle(origin)
{
    private bool _identityPassed;
    private bool _kindPassed;
    private bool _brandPassed;
    private bool _locationPassed;
    private bool _pricePassed;
    private bool _photosPassed;
    private bool _ctxesPassed;

    protected override VehicleIdentity Identity
    {
        get
        {
            if (_identityPassed)
                return base.Identity;
            VehicleIdentity identity = base.Identity;
            if (!identity)
                throw new ValueNotValidException("Идентификатор техники");
            _identityPassed = true;
            return identity;
        }
    }

    protected override VehicleKind Kind
    {
        get
        {
            if (_kindPassed)
                return base.Kind;
            VehicleKind kind = base.Kind;
            if (kind is UnknownVehicleKind)
                throw new ValueNotValidException("Тип техники", "не задан.");
            _kindPassed = true;
            return kind;
        }
    }

    protected override VehicleBrand Brand
    {
        get
        {
            if (_brandPassed)
                return base.Brand;
            VehicleBrand brand = base.Brand;
            if (brand is UnknownVehicleBrand)
                throw new ValueNotValidException("Бренд техники", "не задан.");
            _brandPassed = true;
            return brand;
        }
    }

    protected override GeoLocation Location
    {
        get
        {
            if (_locationPassed)
                return base.Location;
            GeoLocation location = base.Location;
            if (location is UnknownGeolocation)
                throw new ValueNotValidException("Локация техники", "не задана.");
            _locationPassed = true;
            return location;
        }
    }

    protected override IItemPrice Price
    {
        get
        {
            if (_pricePassed)
                return base.Price;
            IItemPrice price = base.Price;
            PriceValue value = price.Value();
            if (!value)
                throw new ValueNotValidException("Стоимость техники", "не задана.");
            _pricePassed = true;
            return price;
        }
    }

    public override VehicleCharacteristics Characteristics
    {
        get
        {
            if (_ctxesPassed)
                return base.Characteristics;
            VehicleCharacteristics characteristics = base.Characteristics;
            if (characteristics.Amount() == 0)
                throw new ValueNotValidException("Характеристики техники", "не заданы.");
            _ctxesPassed = true;
            return characteristics;
        }
    }

    protected override VehiclePhotos Photos
    {
        get
        {
            if (_photosPassed)
                return base.Photos;
            VehiclePhotos photos = base.Photos;
            IEnumerable<VehiclePhoto> photo = photos.Read();
            foreach (string entry in photo)
                if (string.IsNullOrWhiteSpace(entry))
                    throw new ValueNotValidException(
                        "Список фотографий содержит",
                        "пустую фотографию"
                    );
            _photosPassed = true;
            return photos;
        }
    }
}
