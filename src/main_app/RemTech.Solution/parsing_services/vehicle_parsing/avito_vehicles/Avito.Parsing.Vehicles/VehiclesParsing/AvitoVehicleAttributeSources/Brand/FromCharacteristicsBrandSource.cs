using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;
using PuppeteerSharp;
using RemTech.Core.Shared.Primitives;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Brand;

public sealed class FromCharacteristicsBrandSource(IPage page) : IParsedVehicleBrandSource
{
    public async Task<ParsedVehicleBrand> Read()
    {
        IElementHandle[] ctxes = await new AvitoCharacteristicsSource(page).Read();
        foreach (IElementHandle ctxe in ctxes)
        {
            string text = await new TextFromWebElement(ctxe).Read();
            if (text.Contains("марка", StringComparison.OrdinalIgnoreCase))
                return new ParsedVehicleBrand(text.Split(':', StringSplitOptions.TrimEntries)[^1].Trim());
        }
        
        return new ParsedVehicleBrand(new NotEmptyString(string.Empty));
    }
}