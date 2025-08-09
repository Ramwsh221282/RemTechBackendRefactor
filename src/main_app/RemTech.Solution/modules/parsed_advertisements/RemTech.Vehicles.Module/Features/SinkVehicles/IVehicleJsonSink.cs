using RemTech.Vehicles.Module.Types.Brands;
using RemTech.Vehicles.Module.Types.Characteristics.Features.Structuring;
using RemTech.Vehicles.Module.Types.GeoLocations;
using RemTech.Vehicles.Module.Types.Kinds;
using RemTech.Vehicles.Module.Types.Models;
using RemTech.Vehicles.Module.Types.Transport;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Prices;

namespace RemTech.Vehicles.Module.Features.SinkVehicles;

public interface IVehicleJsonSink
{
    VehicleKind Kind();
    VehicleBrand Brand();
    VehicleModel Model();
    GeoLocation Location();
    VehicleIdentity VehicleId();
    IItemPrice VehiclePrice();
    VehiclePhotos VehiclePhotos();
    Vehicle Vehicle();
    CharacteristicVeil[] Characteristics();
    string ParserName();
    string ParserType();
    string LinkName();
    string SourceUrl();
    string SourceDomain();
    string Description();
}
