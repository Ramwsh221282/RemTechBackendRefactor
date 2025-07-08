namespace RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices;

public interface IParsedItemPrice
{
    PriceValue Value();
    bool UnderNds();
}
