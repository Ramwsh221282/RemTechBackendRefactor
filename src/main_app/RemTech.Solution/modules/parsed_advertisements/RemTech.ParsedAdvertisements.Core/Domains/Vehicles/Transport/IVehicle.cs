using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

public interface IVehicle
{
    IVehicleKind Kind();
    IVehicleBrand Brand();
    IGeoLocation Location();
    VehicleIdentity Identity();
    IItemPrice Cost();
    VehicleText TextInformation();
    VehiclePhotos Photos();
    VehicleCharacteristics Characteristics();
}
