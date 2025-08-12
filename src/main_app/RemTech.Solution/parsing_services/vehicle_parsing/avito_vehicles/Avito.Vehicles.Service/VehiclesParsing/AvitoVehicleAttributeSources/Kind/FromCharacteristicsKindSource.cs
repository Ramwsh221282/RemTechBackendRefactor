using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using PuppeteerSharp;
using RemTech.Core.Shared.Exceptions;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Kind;

public sealed class FromCharacteristicsKindSource(IPage page) : IParsedVehicleKindSource
{
    private const string Type = "тип техники";
    private const char SplitChar = ':';

    public async Task<ParsedVehicleKind> Read()
    {
        IElementHandle[] ctxes = await new AvitoCharacteristicsSource(page).Read();
        foreach (IElementHandle ctxe in ctxes)
        {
            string text = await new TextFromWebElement(ctxe).Read();
            if (text.Contains(Type, StringComparison.OrdinalIgnoreCase))
                return new ParsedVehicleKind(
                    text.Split(SplitChar, StringSplitOptions.TrimEntries)[^1].Trim()
                );
        }

        throw new OperationException("Unable to find transport type from characteristics element.");
    }
}
