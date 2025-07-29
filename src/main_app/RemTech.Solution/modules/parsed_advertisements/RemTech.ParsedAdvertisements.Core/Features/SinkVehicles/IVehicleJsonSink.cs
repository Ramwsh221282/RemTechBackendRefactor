using RemTech.ParsedAdvertisements.Core.Types.Brands;
using RemTech.ParsedAdvertisements.Core.Types.Characteristics.Features.Structuring;
using RemTech.ParsedAdvertisements.Core.Types.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Types.Kinds;
using RemTech.ParsedAdvertisements.Core.Types.Models;
using RemTech.ParsedAdvertisements.Core.Types.Transport;
using RemTech.ParsedAdvertisements.Core.Types.Transport.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Types.Transport.ValueObjects.Prices;

namespace RemTech.ParsedAdvertisements.Core.Features.SinkVehicles;

public interface IVehicleJsonSink : IDisposable
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
}
