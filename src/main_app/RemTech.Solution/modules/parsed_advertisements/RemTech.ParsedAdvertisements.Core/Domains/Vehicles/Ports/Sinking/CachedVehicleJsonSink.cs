using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Ports.Sinking;

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
    private UnstructuredCharacteristic[]? _characteristics;

    public CachedVehicleJsonSink(IVehicleJsonSink origin)
    {
        _origin = origin;
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

    public UnstructuredCharacteristic[] Characteristics()
    {
        _characteristics ??= _origin.Characteristics();
        return _characteristics;
    }

    public void Dispose()
    {
        _origin.Dispose();
    }
}