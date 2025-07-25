using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePrices;
using RemTech.Core.Shared.Primitives;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Price;

public sealed class DefaultVehiclePriceOnErrorSource(IParsedVehiclePriceSource source) : IParsedVehiclePriceSource
{
    public async Task<ParsedVehiclePrice> Read()
    {
        try
        {
            return await source.Read();
        }
        catch
        {
            return new ParsedVehiclePrice(new PositiveLong(-1), new NotEmptyString(string.Empty));
        }
    }
}