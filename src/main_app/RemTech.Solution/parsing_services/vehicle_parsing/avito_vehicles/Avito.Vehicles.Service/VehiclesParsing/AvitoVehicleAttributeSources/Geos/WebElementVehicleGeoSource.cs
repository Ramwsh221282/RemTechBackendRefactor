using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;
using Parsing.Vehicles.Common.TextWriting;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Geos;

public sealed class WebElementVehicleGeoSource(IPage page, ITextWrite write)
    : IParsedVehicleGeoSource
{
    private readonly string _geoContainerSelector = string.Intern("div[itemprop='address']");
    private readonly string _geoValueSelector = string.Intern(".xLPJ6");
    private readonly StringSplitOptions _splitOptions =
        StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;

    public async Task<ParsedVehicleGeo> Read()
    {
        IElementHandle geoContainer = await new PageElementSource(page).Read(_geoContainerSelector);
        IElementHandle geoValue = await new ParentElementSource(geoContainer).Read(
            _geoValueSelector
        );
        string text = await new TextFromWebElement(geoValue).Read();
        await write.WriteAsync(text);
        string[] textParts = text.Split(',', _splitOptions);
        string regionPart = textParts[0];
        return new ParsedVehicleGeo(regionPart);
    }
}
