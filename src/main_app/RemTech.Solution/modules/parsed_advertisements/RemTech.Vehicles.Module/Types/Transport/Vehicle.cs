using RemTech.Vehicles.Module.Types.Brands;
using RemTech.Vehicles.Module.Types.Brands.Decorators.Logic;
using RemTech.Vehicles.Module.Types.GeoLocations;
using RemTech.Vehicles.Module.Types.GeoLocations.Decorators.Logic;
using RemTech.Vehicles.Module.Types.Kinds;
using RemTech.Vehicles.Module.Types.Kinds.Decorators.Logic;
using RemTech.Vehicles.Module.Types.Models;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Prices;

namespace RemTech.Vehicles.Module.Types.Transport;

public class Vehicle : IVehicle
{
    protected virtual VehicleIdentity Identity { get; }
    protected virtual VehicleKind Kind { get; }
    protected virtual VehicleBrand Brand { get; }
    protected virtual GeoLocation Location { get; }
    protected virtual IItemPrice Price { get; }
    protected virtual VehiclePhotos Photos { get; }
    public string SourceUrl { get; private set; }
    public string Description { get; private set; }
    public string SourceDomain { get; private set; }
    public virtual VehicleCharacteristics Characteristics { get; }
    protected virtual VehicleModel Model { get; }

    public Vehicle(
        VehicleIdentity identity,
        IItemPrice price,
        VehiclePhotos photos,
        string sourceUrl,
        string sourceDomain,
        string description
    )
    {
        SourceUrl = sourceUrl;
        SourceDomain = sourceDomain;
        Identity = identity;
        Price = price;
        Photos = photos;
        Characteristics = new VehicleCharacteristics([]);
        Kind = new UnknownVehicleKind();
        Brand = new UnknownVehicleBrand();
        Location = new UnknownGeolocation();
        Model = new VehicleModel();
        Description = description;
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
        SourceUrl = origin.SourceUrl;
        SourceDomain = origin.SourceDomain;
        Description = origin.Description;
    }
}
