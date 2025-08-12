using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Characteristics;

public sealed class AvitoDescriptionParts(IPage page) : IAvitoDescriptionParts
{
    private const string DescriptionSelector = "div[data-marker='item-view/item-description']";

    public async Task<string> Read()
    {
        IElementHandle element = await new ValidSingleElementSource(
            new PageElementSource(page)
        ).Read(DescriptionSelector);
        return await new InnerTextFromWebElement(element).Read();
    }
}
