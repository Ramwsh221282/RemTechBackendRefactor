using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;
using Parsing.Vehicles.Common.TextWriting;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Geos;

public sealed class WebElementVehicleGeoSource(IPage page, ITextWrite write)
    : IParsedVehicleGeoSource
{
    private const string GeoContainerSelector = "div[itemprop='address']";
    private const string GeoValueSelector = ".xLPJ6";
    private const StringSplitOptions SplitOptions =
        StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
    private const char SplitChar = ',';

    public async Task<ParsedVehicleGeo> Read()
    {
        IElementHandle geoContainer = await new PageElementSource(page).Read(GeoContainerSelector);
        IElementHandle geoValue = await new ParentElementSource(geoContainer).Read(
            GeoValueSelector
        );
        string text = await new TextFromWebElement(geoValue).Read();
        await write.WriteAsync(text);
        string[] textParts = text.Split(SplitChar, SplitOptions);
        string regionPart = textParts[0];
        return new ParsedVehicleGeo(regionPart);
    }
}
