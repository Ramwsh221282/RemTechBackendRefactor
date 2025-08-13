using RemTech.Vehicles.Module.Features.SinkVehicles.Types;
using RemTech.Vehicles.Module.Types.Transport;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Prices;

namespace RemTech.Vehicles.Module.Features.SinkVehicles;

internal sealed class CachedVehicleJsonSink : IVehicleJsonSink
{
    private readonly IVehicleJsonSink _origin;
    private readonly SinkedVehicleCategory _category;
    private readonly SinkedVehicleBrand _brand;
    private readonly SinkedVehicleModel _model;
    private readonly SinkedVehicleLocation _location;
    private VehicleIdentity? _identity;
    private IItemPrice? _cost;
    private VehiclePhotos? _photos;
    private Vehicle? _vehicle;
    private IEnumerable<VehicleBodyCharacteristic> _characteristics;

    public CachedVehicleJsonSink(IVehicleJsonSink origin)
    {
        _origin = origin;
        _category = origin.Category();
        _brand = origin.Brand();
        _model = origin.Model();
        _location = origin.Location();
        _identity = origin.VehicleId();
        _cost = origin.VehiclePrice();
        _photos = origin.VehiclePhotos();
        _vehicle = origin.Vehicle();
        _characteristics = origin.Characteristics();
    }

    public CachedVehicleJsonSink(IVehicleJsonSink origin, SinkedVehicleCategory category)
        : this(origin)
    {
        _category = category;
    }

    public CachedVehicleJsonSink(IVehicleJsonSink origin, SinkedVehicleBrand brand)
        : this(origin)
    {
        _brand = brand;
    }

    public CachedVehicleJsonSink(IVehicleJsonSink origin, SinkedVehicleModel model)
        : this(origin)
    {
        _model = model;
    }

    public CachedVehicleJsonSink(IVehicleJsonSink origin, SinkedVehicleLocation location)
        : this(origin)
    {
        _location = location;
    }

    public CachedVehicleJsonSink(IVehicleJsonSink origin, VehicleIdentity identity)
        : this(origin)
    {
        _identity = identity;
    }

    public CachedVehicleJsonSink(IVehicleJsonSink origin, IItemPrice price)
        : this(origin)
    {
        _cost = price;
    }

    public CachedVehicleJsonSink(IVehicleJsonSink origin, VehiclePhotos photos)
        : this(origin)
    {
        _photos = photos;
    }

    public CachedVehicleJsonSink(IVehicleJsonSink origin, Vehicle vehicle)
        : this(origin)
    {
        _vehicle = vehicle;
    }

    public CachedVehicleJsonSink(
        IVehicleJsonSink origin,
        IEnumerable<VehicleBodyCharacteristic> characteristics
    )
        : this(origin)
    {
        _characteristics = characteristics;
    }

    public CachedVehicleJsonSink(CachedVehicleJsonSink origin)
    {
        _origin = origin;
        _category = origin._category;
        _brand = origin._brand;
        _model = origin._model;
        _location = origin._location;
        _identity = origin._identity;
        _cost = origin._cost;
        _photos = origin._photos;
        _vehicle = origin._vehicle;
        _characteristics = origin._characteristics;
    }

    public SinkedVehicleCategory Category()
    {
        return _category;
    }

    SinkedVehicleBrand IVehicleJsonSink.Brand()
    {
        return _brand;
    }

    SinkedVehicleModel IVehicleJsonSink.Model()
    {
        return _model;
    }

    SinkedVehicleLocation IVehicleJsonSink.Location()
    {
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

    public IEnumerable<VehicleBodyCharacteristic> Characteristics()
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

    public string SourceUrl()
    {
        return _origin.SourceUrl();
    }

    public string SourceDomain()
    {
        return _origin.SourceDomain();
    }

    public string Sentences()
    {
        return _origin.Sentences();
    }
}
