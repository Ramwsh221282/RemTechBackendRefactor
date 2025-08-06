using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePrices;
using RemTech.Core.Shared.Primitives;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Price;

public sealed class VarietAvitoVehiclePriceSource : IParsedVehiclePriceSource
{
    private readonly List<IParsedVehiclePriceSource> _variants;

    public VarietAvitoVehiclePriceSource()
    {
        _variants = [];
    }

    public VarietAvitoVehiclePriceSource With(IParsedVehiclePriceSource source)
    {
        _variants.Add(source);
        return this;
    }

    public async Task<ParsedVehiclePrice> Read()
    {
        foreach (IParsedVehiclePriceSource source in _variants)
        {
            ParsedVehiclePrice price = await source.Read();
            if (price)
                return price;
        }

        return new ParsedVehiclePrice(new PositiveLong(-1), new NotEmptyString(string.Empty));
    }
}
