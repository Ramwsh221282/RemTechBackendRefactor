using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Bakery.Receipts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Bakery;

public interface IVehicleKindBakery
{
    Status<IVehicleKind> Baked(IVehicleKindReceipt receipt);
}
