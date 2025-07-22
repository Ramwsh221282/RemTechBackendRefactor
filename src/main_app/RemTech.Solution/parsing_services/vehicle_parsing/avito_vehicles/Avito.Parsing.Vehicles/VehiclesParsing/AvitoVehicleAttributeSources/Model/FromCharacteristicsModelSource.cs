using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using PuppeteerSharp;
using RemTech.Core.Shared.Primitives;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Model;

public sealed class FromCharacteristicsModelSource(IPage page) : IParsedVehicleModelSource
{
    public async Task<ParsedVehicleModel> Read()
    {
        IElementHandle[] ctxes = await new AvitoCharacteristicsSource(page).Read();
        foreach (IElementHandle ctxe in ctxes)
        {
            string text = await new TextFromWebElement(ctxe).Read();
            if (text.Contains("модель", StringComparison.OrdinalIgnoreCase))
                return new ParsedVehicleModel(text.Split(':', StringSplitOptions.TrimEntries)[^1].Trim());
        }
        
        return new ParsedVehicleModel(new NotEmptyString(string.Empty));
    }
}