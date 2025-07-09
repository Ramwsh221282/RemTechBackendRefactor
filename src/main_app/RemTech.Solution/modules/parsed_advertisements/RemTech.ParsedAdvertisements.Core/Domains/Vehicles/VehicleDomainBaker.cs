using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Bakery;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Bakery.Receipts;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Bakery;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Bakery.Receipts;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Bakery;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Bakery.Receipts;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Bakery;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Bakery.Receipts;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Bakery;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles;

public sealed class VehicleDomainBaker
{
    public Status<IVehicleBrand> Baked(
        IVehicleBrandBakery brandBakery,
        IVehicleBrandReceipt receipt
    ) => brandBakery.Bake(receipt);

    public Status<IGeoLocation> Baked(IVehicleGeoBakery bakery, IVehicleGeoReceipt receipt) =>
        bakery.Baked(receipt);

    public Status<IVehicleKind> Baked(IVehicleKindBakery bakery, IVehicleKindReceipt receipt) =>
        bakery.Baked(receipt);

    public Status<ICharacteristic> Baked(
        ICharacteristicBakery bakery,
        ICharacteristicReceipt receipt
    ) => bakery.Baked(receipt);

    public Status<IVehicle> Baked(IVehicleBakery bakery, IVehicleReceipt receipt) =>
        bakery.Baked(receipt);
}
