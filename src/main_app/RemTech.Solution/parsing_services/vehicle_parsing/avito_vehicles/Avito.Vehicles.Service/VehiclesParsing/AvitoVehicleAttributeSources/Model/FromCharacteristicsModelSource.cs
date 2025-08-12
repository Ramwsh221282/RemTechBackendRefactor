using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using PuppeteerSharp;
using RemTech.Core.Shared.Primitives;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Model;

public sealed class FromCharacteristicsModelSource(IPage page) : IParsedVehicleModelSource
{
    private const string Model = "модель";
    private const char SplitChar = ':';
    private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;
    private const StringSplitOptions SplitOptions = StringSplitOptions.TrimEntries;

    public async Task<ParsedVehicleModel> Read()
    {
        IElementHandle[] ctxes = await new AvitoCharacteristicsSource(page).Read();
        foreach (IElementHandle ctxe in ctxes)
        {
            string text = await new TextFromWebElement(ctxe).Read();
            if (text.Contains(Model, Comparison))
                return new ParsedVehicleModel(text.Split(SplitChar, SplitOptions)[^1].Trim());
        }

        return new ParsedVehicleModel(new NotEmptyString(string.Empty));
    }
}
