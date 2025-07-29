using RemTech.Core.Shared.Primitives;

namespace RemTech.Vehicles.Module.Types.Transport.ValueObjects.Prices;

public readonly record struct ItemPriceWithNds : IItemPrice
{
    private readonly PriceValue _value;

    public ItemPriceWithNds(PriceValue value)
    {
        _value = value;
    }

    public PriceValue Value()
    {
        return _value;
    }

    public bool UnderNds()
    {
        return true;
    }

    public static implicit operator PositiveLong(ItemPriceWithNds itemPrice)
    {
        return itemPrice._value;
    }

    public static implicit operator long(ItemPriceWithNds itemPrice)
    {
        return itemPrice._value;
    }

    public static implicit operator bool(ItemPriceWithNds itemPrice)
    {
        return itemPrice._value;
    }
}
