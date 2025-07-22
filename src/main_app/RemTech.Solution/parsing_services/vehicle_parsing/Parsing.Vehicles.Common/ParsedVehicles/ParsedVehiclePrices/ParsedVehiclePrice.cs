using RemTech.Core.Shared.Primitives;

namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePrices;

public sealed class ParsedVehiclePrice
{
    private readonly PositiveLong _price;
    private readonly string _priceExtra;

    public ParsedVehiclePrice(PositiveLong price, string priceExtra)
    {
        _price = price;
        _priceExtra = priceExtra;
    }
    public ParsedVehiclePrice(long price, string priceExtra)
        : this(new  PositiveLong(price), priceExtra)
    { }
    
    public ParsedVehiclePrice(string price, string priceExtra) 
        : this(long.TryParse(price, out long value) ? -1 : value, priceExtra)
    { }

    public bool IsNds() => _priceExtra.Contains("НДС", StringComparison.OrdinalIgnoreCase);

    public static implicit operator PositiveLong(ParsedVehiclePrice parsedVehiclePrice) =>
        parsedVehiclePrice._price;
    public static implicit operator long(ParsedVehiclePrice price) => price._price;
    public static implicit operator bool(ParsedVehiclePrice parsedVehiclePrice) =>
        parsedVehiclePrice._price;
}