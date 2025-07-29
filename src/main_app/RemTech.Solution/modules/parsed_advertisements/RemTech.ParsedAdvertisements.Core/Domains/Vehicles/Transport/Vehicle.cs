using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Decorators;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Decorators.Logic;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators.Logic;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators.Logic;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

public class Vehicle : IVehicle
{
    protected virtual VehicleIdentity Identity { get; }
    protected virtual VehicleKind Kind { get; }
    protected virtual VehicleBrand Brand { get; }
    protected virtual GeoLocation Location { get; }
    protected virtual IItemPrice Price { get; }
    protected virtual VehiclePhotos Photos { get; }
    public virtual VehicleCharacteristics Characteristics { get; }
    protected virtual VehicleModel Model { get; }

    public Vehicle(VehicleIdentity identity, IItemPrice price, VehiclePhotos photos)
    {
        Identity = identity;
        Price = price;
        Photos = photos;
        Characteristics = new VehicleCharacteristics([]);
        Kind = new UnknownVehicleKind();
        Brand = new UnknownVehicleBrand();
        Location = new UnknownGeolocation();
        Model = new VehicleModel();
    }

    public string Id() => Identity.Read();

    public Vehicle(Vehicle origin, IEnumerable<VehicleCharacteristic> characteristics)
        : this(origin)
    {
        Characteristics = new VehicleCharacteristics(characteristics);
    }

    public Vehicle(Vehicle origin, VehicleKind kind)
        : this(origin) => Kind = kind;

    public Vehicle(Vehicle origin, VehicleBrand brand)
        : this(origin) => Brand = brand;

    public Vehicle(Vehicle origin, GeoLocation location)
        : this(origin) => Location = location;

    public Vehicle(Vehicle origin, VehicleModel model)
        : this(origin) => Model = model;

    public Vehicle(Vehicle origin, VehicleCharacteristic ctx)
        : this(origin)
    {
        VehicleCharacteristic[] current = origin.Characteristics.Read();
        Characteristics = new VehicleCharacteristics([.. current, ctx]);
    }

    public Vehicle(Vehicle origin)
    {
        Identity = origin.Identity;
        Kind = origin.Kind;
        Brand = origin.Brand;
        Location = origin.Location;
        Price = origin.Price;
        Photos = origin.Photos;
        Characteristics = origin.Characteristics;
        Model = origin.Model;
    }
}
