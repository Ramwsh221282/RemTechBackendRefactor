using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices.Bakeries.Receipts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices.Bakeries;

public interface IItemPriceBakery
{
    Status<IItemPrice> Baked(IItemPriceReceipt receipt);
}
