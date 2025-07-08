using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles;

public sealed class VehicleContainer
{
    private readonly ParsedTransport _transport;
    private readonly ParsedVehicleKind _kind;
    private readonly ParsedVehicleBrand _brand;
    private readonly ParsedGeoLocation _geo;
    private readonly ParsedVehicleCharacteristic[] _characteristics;

    public VehicleContainer(
        ParsedTransport transport,
        ParsedVehicleKind kind,
        ParsedVehicleBrand brand,
        ParsedGeoLocation geo,
        ParsedVehicleCharacteristic[] ctx
    )
    {
        _transport = transport;
        _kind = kind;
        _brand = brand;
        _geo = geo;
        _characteristics = ctx;
    }

    public ContainedVehicle Contain()
    {
        return new ContainedVehicle(_transport, _kind, _brand, _geo, _characteristics);
    }
}
