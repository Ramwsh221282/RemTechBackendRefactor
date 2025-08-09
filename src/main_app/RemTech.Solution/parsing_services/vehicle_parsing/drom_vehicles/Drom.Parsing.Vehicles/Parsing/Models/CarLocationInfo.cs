namespace Drom.Parsing.Vehicles.Parsing.Models;

public sealed class CarLocationInfo(string locationInfo)
{
    public void Print(DromCatalogueCar car)
    {
        car.WithLocation(locationInfo);
    }
}
