namespace Drom.Parsing.Vehicles.Parsing.Models;

public sealed class CarPrice
{
    private readonly long _value;
    private readonly bool _isNds;

    public CarPrice(long value, bool isNds)
    {
        _value = value;
        _isNds = isNds;
    }

    public void Print(DromCatalogueCar car)
    {
        car.WithPrice(_value, _isNds);
    }
}
