using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Bakery.Receipts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Bakery;

public interface IVehicleBrandBakery
{
    public Status<IVehicleBrand> Bake(IVehicleBrandReceipt receipt);
}
