using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Decorators;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

public sealed class Vehicle : IVehicle
{
    private readonly VehicleIdentity _identity;
    private readonly IVehicleKind _kind;
    private readonly IVehicleBrand _brand;
    private readonly IGeoLocation _location;
    private readonly IItemPrice _price;
    private readonly VehiclePhotos _photos;
    private readonly VehicleCharacteristics _characteristics;
    
    public IVehicleKind Kind() => _kind;

    public IVehicleBrand Brand() => _brand;

    public IGeoLocation Location() => _location;

    public VehicleIdentity Identity() => _identity;

    public IItemPrice Cost() => _price;

    public VehiclePhotos Photos() => _photos;

    public VehicleCharacteristics Characteristics() => _characteristics;
    
    public Vehicle(Vehicle origin, IVehicleKind kind)
    : this (origin) => _kind = kind;

    public Vehicle(Vehicle origin, IVehicleBrand brand)
    : this(origin) => _brand = brand;

    public Vehicle(Vehicle origin, IGeoLocation location)
    : this(origin) => _location = location;

    public Vehicle(VehicleIdentity identity, IItemPrice price, VehiclePhotos photos)
    {
        _identity = identity;
        _price = price;
        _photos = photos;
        _characteristics = new VehicleCharacteristics([]);
        _kind = new UnknownVehicleKind();
        _brand = new UnknownVehicleBrand();
        _location = new UnknownGeolocation();
    }
    
    public Vehicle(VehicleIdentity identity, IItemPrice price, VehiclePhotos photos, VehicleCharacteristics characteristics)
    {
        _identity = identity;
        _price = price;
        _photos = photos;
        _characteristics = characteristics;
        _kind = new UnknownVehicleKind();
        _brand = new UnknownVehicleBrand();
        _location = new UnknownGeolocation();
    }
    
    public Vehicle(VehicleIdentity identity,
        IVehicleKind kind,
        IVehicleBrand brand,
        IGeoLocation location,
        IItemPrice price,
        VehiclePhotos photos,
        VehicleCharacteristics characteristics)
    {
        _identity = identity;
        _kind = kind;
        _brand = brand;
        _price = price;
        _photos = photos;
        _characteristics = characteristics;
        _location = location;
    }
    
    public Vehicle(IVehicle origin)
        : this(
            origin.Identity(),
            origin.Kind(),
            origin.Brand(),
            origin.Location(),
            origin.Cost(),
            origin.Photos(),
            origin.Characteristics()
        ) { }
    
    public Vehicle(Vehicle origin)
    {
        _identity = origin._identity;
        _kind = origin._kind;
        _brand = origin._brand;
        _location = origin._location;
        _price = origin._price;
        _photos = origin._photos;
        _characteristics = origin._characteristics;
    }
}
