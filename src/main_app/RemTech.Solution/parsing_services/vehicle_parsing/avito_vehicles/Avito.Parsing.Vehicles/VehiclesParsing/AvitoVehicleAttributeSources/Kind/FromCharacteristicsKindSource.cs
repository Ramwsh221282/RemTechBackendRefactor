using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using PuppeteerSharp;
using RemTech.Core.Shared.Exceptions;
using RemTech.Core.Shared.Primitives;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Kind;

public sealed class FromCharacteristicsKindSource(IPage page) : IParsedVehicleKindSource
{
    public async Task<ParsedVehicleKind> Read()
    {
        IElementHandle[] ctxes = await new AvitoCharacteristicsSource(page).Read();
        foreach (IElementHandle ctxe in ctxes)
        {
            string text = await new TextFromWebElement(ctxe).Read();
            if (text.Contains("тип техники", StringComparison.OrdinalIgnoreCase))
                return new ParsedVehicleKind(
                    text.Split(':', StringSplitOptions.TrimEntries)[^1].Trim()
                );
        }

        throw new OperationException("Unable to find transport type from characteristics element.");
    }
}
