namespace RemTech.ParsedAdvertisements.Core.Common.ParsedItemPrices;

public interface IParsedItemPrice
{
    PriceValue Value();
    bool UnderNds();
}
