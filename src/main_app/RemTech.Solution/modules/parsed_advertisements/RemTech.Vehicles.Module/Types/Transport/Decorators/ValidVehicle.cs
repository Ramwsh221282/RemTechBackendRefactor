using RemTech.Core.Shared.Exceptions;
using RemTech.Vehicles.Module.Features.SinkVehicles.Types;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Prices;

namespace RemTech.Vehicles.Module.Types.Transport.Decorators;

internal sealed class ValidVehicle(Vehicle origin) : Vehicle(origin)
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

    protected override SinkedVehicleCategory Category
    {
        get
        {
            if (_kindPassed)
                return base.Category;
            SinkedVehicleCategory category = base.Category;
            if (string.IsNullOrWhiteSpace(category.Name) || category.Id == Guid.Empty)
                throw new ValueNotValidException("Категория не распознана.");
            _kindPassed = true;
            return category;
        }
    }

    protected override SinkedVehicleBrand Brand
    {
        get
        {
            if (_brandPassed)
                return base.Brand;
            SinkedVehicleBrand brand = base.Brand;
            if (string.IsNullOrWhiteSpace(brand.Name) || brand.Id == Guid.Empty)
                throw new ValueNotValidException("Бренд не распознан.");
            _brandPassed = true;
            return brand;
        }
    }

    protected override SinkedVehicleLocation Location
    {
        get
        {
            if (_locationPassed)
                return base.Location;
            SinkedVehicleLocation location = base.Location;
            if (
                string.IsNullOrWhiteSpace(location.Text)
                || string.IsNullOrWhiteSpace(location.KindText)
                || string.IsNullOrWhiteSpace(location.CityText)
                || location.Id == Guid.Empty
            )
                throw new ValueNotValidException("Локация не распознана.");
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
