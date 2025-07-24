using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Ports.Sinking;

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
    UnstructuredCharacteristic[] Characteristics();
}