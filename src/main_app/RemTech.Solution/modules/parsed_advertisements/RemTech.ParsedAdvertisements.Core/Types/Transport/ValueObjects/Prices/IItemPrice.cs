namespace RemTech.ParsedAdvertisements.Core.Types.Transport.ValueObjects.Prices;

public interface IItemPrice
{
    PriceValue Value();
    bool UnderNds();
}
