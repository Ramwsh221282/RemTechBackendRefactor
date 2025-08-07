using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePrices;
using PuppeteerSharp;
using RemTech.Core.Shared.Primitives;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Price;

public sealed class AvitoPriceFromContainer(IPage page) : IParsedVehiclePriceSource
{
    private readonly string _containerSelector = string.Intern(
        "div[data-marker='item-view/item-price-container']"
    );
    private readonly string _priceValueSelector = string.Intern(
        "span[data-marker='item-view/item-price']"
    );
    private readonly string _extraSelector = string.Intern(".JyWQn");
    private readonly string _priceValueAttribute = string.Intern("content");

    public async Task<ParsedVehiclePrice> Read()
    {
        IElementHandle priceContainer = await new ValidSingleElementSource(
            new PageElementSource(page)
        ).Read(_containerSelector);

        IElementHandle priceElement = await new ValidSingleElementSource(
            new ParentElementSource(priceContainer)
        ).Read(_priceValueSelector);

        if (
            !long.TryParse(
                await new AttributeFromWebElement(priceElement, _priceValueAttribute).Read(),
                out long price
            )
        )
            return new ParsedVehiclePrice(new PositiveLong(-1), new NotEmptyString(string.Empty));

        IElementHandle priceExtra = await new ValidSingleElementSource(
            new ParentElementSource(priceContainer)
        ).Read(_extraSelector);

        string extraText = await new TextFromWebElement(priceExtra).Read();
        return new ParsedVehiclePrice(new PositiveLong(price), extraText);
    }
}
