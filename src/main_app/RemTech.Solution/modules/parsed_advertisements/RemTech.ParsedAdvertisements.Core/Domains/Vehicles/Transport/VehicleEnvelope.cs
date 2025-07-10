using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

public abstract class VehicleEnvelope(
    VehicleIdentity identity,
    IVehicleKind kind,
    IVehicleBrand brand,
    IGeoLocation location,
    IItemPrice price,
    VehicleText text,
    VehiclePhotos photos,
    VehicleCharacteristics characteristics
) : IVehicle
{
    private readonly VehicleIdentity _identity = identity;
    private readonly IVehicleKind _kind = kind;
    private readonly IVehicleBrand _brand = brand;
    private readonly IGeoLocation _location = location;
    private readonly IItemPrice _price = price;
    private readonly VehicleText _text = text;
    private readonly VehiclePhotos _photos = photos;
    private readonly VehicleCharacteristics _characteristics = characteristics;

    public VehicleEnvelope(IVehicle origin)
        : this(
            origin.Identity(),
            origin.Kind(),
            origin.Brand(),
            origin.Location(),
            origin.Cost(),
            origin.TextInformation(),
            origin.Photos(),
            origin.Characteristics()
        ) { }

    public IVehicleKind Kind() => _kind;

    public IVehicleBrand Brand() => _brand;

    public IGeoLocation Location() => _location;

    public VehicleIdentity Identity() => _identity;

    public IItemPrice Cost() => _price;

    public VehicleText TextInformation() => _text;

    public VehiclePhotos Photos() => _photos;

    public VehicleCharacteristics Characteristics() => _characteristics;
}
