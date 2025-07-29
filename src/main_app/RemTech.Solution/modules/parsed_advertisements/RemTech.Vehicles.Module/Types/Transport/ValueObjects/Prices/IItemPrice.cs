namespace RemTech.Vehicles.Module.Types.Transport.ValueObjects.Prices;

public interface IItemPrice
{
    PriceValue Value();
    bool UnderNds();
}
