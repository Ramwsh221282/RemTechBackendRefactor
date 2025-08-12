using RemTech.Vehicles.Module.Features.SinkVehicles.Types;
using RemTech.Vehicles.Module.Types.Characteristics.Features.Structuring;
using RemTech.Vehicles.Module.Types.Transport;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Prices;

namespace RemTech.Vehicles.Module.Features.SinkVehicles;

internal interface IVehicleJsonSink
{
    SinkedVehicleCategory Category();
    SinkedVehicleBrand Brand();
    SinkedVehicleModel Model();
    SinkedVehicleLocation Location();
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
    string Sentences();
}
