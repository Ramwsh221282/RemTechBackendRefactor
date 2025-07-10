namespace RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices;

public interface IItemPrice
{
    PriceValue Value();
    bool UnderNds();
}
