using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Bakery.Receipts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Bakery;

public interface ICharacteristicBakery
{
    public Status<ICharacteristic> Baked(ICharacteristicReceipt receipt);
}
