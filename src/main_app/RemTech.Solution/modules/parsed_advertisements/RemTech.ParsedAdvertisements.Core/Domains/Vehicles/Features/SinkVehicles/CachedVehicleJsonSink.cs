using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.SinkVehicles;

public sealed class CachedVehicleJsonSink  : IVehicleJsonSink
{
    private readonly IVehicleJsonSink _origin;
    private VehicleKind? _kind;
    private VehicleBrand? _brand;
    private VehicleModel? _model;
    private GeoLocation? _location;
    private VehicleIdentity? _identity;
    private IItemPrice? _cost;
    private VehiclePhotos? _photos;
    private Vehicle? _vehicle;
    private CharacteristicVeil[]? _characteristics;

    public CachedVehicleJsonSink(IVehicleJsonSink origin)
    {
        _origin = origin;
    }

    public CachedVehicleJsonSink(IVehicleJsonSink origin, VehicleKind kind) : this(origin)
    {
        _kind = kind;
    }

    public CachedVehicleJsonSink(IVehicleJsonSink origin, VehicleBrand brand) : this(origin)
    {
        _brand = brand;
    }

    public CachedVehicleJsonSink(IVehicleJsonSink origin, VehicleModel model) : this(origin)
    {
        _model = model;
    }

    public CachedVehicleJsonSink(IVehicleJsonSink origin, GeoLocation location) : this(origin)
    {
        _location = location;
    }

    public CachedVehicleJsonSink(IVehicleJsonSink origin, VehicleIdentity identity) : this(origin)
    {
        _identity = identity;
    }

    public CachedVehicleJsonSink(IVehicleJsonSink origin, IItemPrice price) : this(origin)
    {
        _cost = price;
    }

    public CachedVehicleJsonSink(IVehicleJsonSink origin, VehiclePhotos photos) : this(origin)
    {
        _photos = photos;
    }

    public CachedVehicleJsonSink(IVehicleJsonSink origin, Vehicle vehicle) : this(origin)
    {
        _vehicle = vehicle;
    }

    public CachedVehicleJsonSink(IVehicleJsonSink origin, CharacteristicVeil[] characteristics) : this(origin)
    {
        _characteristics = characteristics;
    }

    public CachedVehicleJsonSink(CachedVehicleJsonSink origin)
    {
        _origin = origin;
        _kind = origin._kind;
        _brand = origin._brand;
        _model = origin._model;
        _location = origin._location;
        _identity = origin._identity;
        _cost = origin._cost;
        _photos = origin._photos;
        _vehicle = origin._vehicle;
        _characteristics = origin._characteristics;
    }
    
    public VehicleKind Kind()
    {
        _kind ??= _origin.Kind();
        return _kind;
    }

    public VehicleBrand Brand()
    {
        _brand ??= _origin.Brand();
        return _brand;
    }

    public VehicleModel Model()
    {
        _model ??= _origin.Model();
        return _model;
    }

    public GeoLocation Location()
    {
        _location ??= _origin.Location();
        return _location;
    }

    public VehicleIdentity VehicleId()
    {
        _identity ??= _origin.VehicleId();
        return _identity;
    }

    public IItemPrice VehiclePrice()
    {
        _cost ??= _origin.VehiclePrice();
        return _cost;
    }

    public VehiclePhotos VehiclePhotos()
    {
        _photos ??= _origin.VehiclePhotos();
        return _photos;
    }

    public Vehicle Vehicle()
    {
        _vehicle ??= _origin.Vehicle();
        return _vehicle;
    }

    public CharacteristicVeil[] Characteristics()
    {
        _characteristics ??= _origin.Characteristics();
        return _characteristics;
    }

    public string ParserName()
    {
        return _origin.ParserName();
    }

    public string ParserType()
    {
        return _origin.ParserType();
    }

    public string LinkName()
    {
        return _origin.LinkName();
    }

    public void Dispose()
    {
        _origin.Dispose();
    }
}