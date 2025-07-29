using RemTech.Core.Shared.Primitives;

namespace RemTech.Vehicles.Module.Types.Transport.ValueObjects.Prices;

public readonly record struct ItemPriceWithoutNds : IItemPrice
{
    private readonly PriceValue _value;

    public ItemPriceWithoutNds(PriceValue value)
    {
        _value = value;
    }

    public PriceValue Value()
    {
        return _value;
    }

    public bool UnderNds()
    {
        return false;
    }

    public static implicit operator PositiveLong(ItemPriceWithoutNds itemPrice)
    {
        return itemPrice._value;
    }

    public static implicit operator long(ItemPriceWithoutNds itemPrice)
    {
        return itemPrice._value;
    }

    public static implicit operator bool(ItemPriceWithoutNds itemPrice)
    {
        return itemPrice._value;
    }
}
