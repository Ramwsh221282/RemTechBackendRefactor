namespace RemTech.Vehicles.Module.Types.Transport.ValueObjects.Prices;

public readonly record struct ItemPrice : IItemPrice
{
    private readonly IItemPrice _price;

    public ItemPrice(IItemPrice price)
    {
        _price = price;
    }

    public PriceValue Value()
    {
        return _price.Value();
    }

    public bool UnderNds()
    {
        return _price.UnderNds();
    }
}
