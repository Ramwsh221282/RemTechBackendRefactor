using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePrices;
using PuppeteerSharp;
using RemTech.Core.Shared.Primitives;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Price;

public sealed class AvitoVehiclePriceSource(IPage page) : IParsedVehiclePriceSource
{
    private const string Selector = "#bx_item-price-value";
    private const string PriceValueSelector = "span[data-marker='item-view/item-price']";
    private const string ExtraSelector = ".JyWQn";
    private const string PriceValueAttribute = "content";

    public async Task<ParsedVehiclePrice> Read()
    {
        IElementHandle priceContainer = await new ValidSingleElementSource(
            new PageElementSource(page)
        ).Read(Selector);

        IElementHandle priceValue = await new ValidSingleElementSource(
            new ParentElementSource(priceContainer)
        ).Read(PriceValueSelector);

        if (
            !long.TryParse(
                await new AttributeFromWebElement(priceValue, PriceValueAttribute).Read(),
                out long price
            )
        )
            return new ParsedVehiclePrice(new PositiveLong(-1), new NotEmptyString(string.Empty));

        IElementHandle priceExtra = await new ValidSingleElementSource(
            new ParentElementSource(priceContainer)
        ).Read(ExtraSelector);

        string extraText = await new TextFromWebElement(priceExtra).Read();
        return new ParsedVehiclePrice(new PositiveLong(price), extraText);
    }
}
