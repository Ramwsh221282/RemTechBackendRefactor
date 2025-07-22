using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePrices;
using PuppeteerSharp;
using RemTech.Core.Shared.Primitives;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Price;

public sealed class AvitoVehiclePriceSource(IPage page) : IParsedVehiclePriceSource
{
    private readonly string _selector = string.Intern("#bx_item-price-value");
    private readonly string _priceValueSelector = string.Intern("span[data-marker='item-view/item-price']");
    private readonly string _extraSelector = string.Intern(".JyWQn");
    private readonly string _priceValueAttribute = string.Intern("content");
    
    public async Task<ParsedVehiclePrice> Read()
    {
        IElementHandle priceContainer = await new ValidSingleElementSource(new PageElementSource(page))
            .Read(_selector);
        
        IElementHandle priceValue = await new ValidSingleElementSource(new ParentElementSource(priceContainer))
            .Read(_priceValueSelector);
        
        if (!long.TryParse(
                await new AttributeFromWebElement(priceValue, _priceValueAttribute).Read(),
                out long price))
            return new ParsedVehiclePrice(new PositiveLong(-1), new NotEmptyString(string.Empty));
        
        IElementHandle priceExtra = await new ValidSingleElementSource(new ParentElementSource(priceContainer))
            .Read(_extraSelector);
        
        string extraText = await new TextFromWebElement(priceExtra).Read();
        return new ParsedVehiclePrice(new PositiveLong(price), extraText);
    }
}