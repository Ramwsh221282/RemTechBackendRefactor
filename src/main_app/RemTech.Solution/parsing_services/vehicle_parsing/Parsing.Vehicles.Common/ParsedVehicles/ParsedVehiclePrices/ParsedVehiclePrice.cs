namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePrices;

public sealed class ParsedVehiclePrice(long price, string priceExtra)
{
    private readonly long _price = price;

    public ParsedVehiclePrice(string price, string priceExtra)
        : this(long.TryParse(price, out long value) ? -1 : value, priceExtra) { }

    public bool IsNds() => priceExtra.Contains("НДС", StringComparison.OrdinalIgnoreCase);

    public static implicit operator long(ParsedVehiclePrice price) => price._price;

    public static implicit operator bool(ParsedVehiclePrice parsedVehiclePrice) =>
        parsedVehiclePrice._price != -1;
}
