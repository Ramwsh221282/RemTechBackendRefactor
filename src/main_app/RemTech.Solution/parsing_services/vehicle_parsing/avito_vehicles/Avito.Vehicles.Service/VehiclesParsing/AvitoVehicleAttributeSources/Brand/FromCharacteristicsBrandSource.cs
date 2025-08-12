using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;
using PuppeteerSharp;
using RemTech.Core.Shared.Primitives;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Brand;

public sealed class FromCharacteristicsBrandSource(IPage page) : IParsedVehicleBrandSource
{
    private const string Option = "марка";
    private const char SplitArg = ':';
    private const StringSplitOptions SplitOpts = StringSplitOptions.TrimEntries;
    private const StringComparison StringComp = StringComparison.OrdinalIgnoreCase;

    public async Task<ParsedVehicleBrand> Read()
    {
        IElementHandle[] ctxes = await new AvitoCharacteristicsSource(page).Read();
        foreach (IElementHandle ctxe in ctxes)
        {
            string text = await new TextFromWebElement(ctxe).Read();
            if (text.Contains(Option, StringComp))
                return new ParsedVehicleBrand(text.Split(SplitArg, SplitOpts)[^1].Trim());
        }

        return new ParsedVehicleBrand(new NotEmptyString(string.Empty));
    }
}
