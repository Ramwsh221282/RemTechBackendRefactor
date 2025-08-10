using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.Parsing;

public sealed class AvitoSparesCollection : IAvitoSparesCollection
{
    private readonly IPage _page;
    private string _itemListContainer = string.Intern("#bx_serp-item-list");
    private string _itemListItem = string.Intern("div[data-marker='item']");

    public AvitoSparesCollection(IPage page) => _page = page;

    public async Task<IEnumerable<AvitoSpare>> Read()
    {
        IElementHandle listContainer = await new ValidSingleElementSource(
            new PageElementSource(_page)
        ).Read(_itemListContainer);
        IElementHandle[] items = await new ParentManyElementsSource(listContainer).Read(
            _itemListItem
        );
        LinkedList<AvitoSpare> spares = [];
        foreach (var item in items)
        {
            string id = await ReadId(item);
            string title = await ReadTitle(item);
            string sourceUrl = await ReadSourceUrl(item);
            (long, bool) price = await ReadItemPrice(item);
            if (price.Item1 <= 0)
                continue;
            IElementHandle[] middleBlock = await MiddleBlockData(item);
            string oem = await ReadOem(middleBlock);
            string geo = await ReadGeoInfo(middleBlock);
            string relatedBrand = await ReadRelatedBrand(middleBlock);
            IEnumerable<string> images = await ReadImages(item);
            spares.AddFirst(
                new AvitoSpare(
                    id,
                    sourceUrl,
                    title,
                    oem,
                    relatedBrand,
                    geo,
                    images,
                    price.Item1,
                    price.Item2
                )
            );
        }

        return spares;
    }

    private async Task<string> ReadId(IElementHandle item)
    {
        string attribute = await new AttributeFromWebElement(item, "data-item-id").Read();
        return attribute;
    }

    private async Task<string> ReadTitle(IElementHandle item)
    {
        IElementHandle titleElement = await new ValidSingleElementSource(
            new ParentElementSource(item)
        ).Read(string.Intern(".iva-item-title-KE8A9"));
        return await new TextFromWebElement(titleElement).Read();
    }

    private async Task<string> ReadSourceUrl(IElementHandle item)
    {
        IElementHandle titleContainer = await new ValidSingleElementSource(
            new ParentElementSource(item)
        ).Read(string.Intern(".iva-item-title-KE8A9"));
        IElementHandle titleElement = await new ValidSingleElementSource(
            new ParentElementSource(titleContainer)
        ).Read("a[data-marker='item-title'");
        string href = await new AttributeFromWebElement(titleElement, "href").Read();
        return href;
    }

    private async Task<(long, bool)> ReadItemPrice(IElementHandle item)
    {
        IElementHandle priceContainer = await new ValidSingleElementSource(
            new ParentElementSource(item)
        ).Read("p[data-marker='item-price'");
        IElementHandle priceMeta = await new ValidSingleElementSource(
            new ParentElementSource(priceContainer)
        ).Read("meta[itemprop='price']");
        string value = await new AttributeFromWebElement(priceMeta, "content").Read();
        string text = await new TextFromWebElement(priceContainer).Read();
        return text.Contains("НДС", StringComparison.OrdinalIgnoreCase)
            ? (long.Parse(value), true)
            : (long.Parse(value), false);
    }

    private async Task<string> ReadGeoInfo(IElementHandle[] blockParts)
    {
        foreach (IElementHandle blockPart in blockParts)
        {
            try
            {
                IElementHandle geoRoot = await new ValidSingleElementSource(
                    new ParentElementSource(blockPart)
                ).Read(string.Intern(".geo-root-BBVai"));
                string text = await new TextFromWebElement(geoRoot).Read();
                return text.Trim();
            }
            catch
            {
                // ignored
            }
        }

        return string.Empty;
    }

    private async Task<string> ReadRelatedBrand(IElementHandle[] blockParts)
    {
        foreach (IElementHandle blockPart in blockParts)
        {
            try
            {
                IElementHandle relatedBrandElement = await new ValidSingleElementSource(
                    new ParentElementSource(blockPart)
                ).Read(string.Intern(".iva-item-text-PvwMY"));
                string text = await new TextFromWebElement(relatedBrandElement).Read();
                return text.Trim();
            }
            catch
            {
                // ignored
            }
        }

        return string.Empty;
    }

    private async Task<string> ReadOem(IElementHandle[] blockParts)
    {
        foreach (IElementHandle blockPart in blockParts)
        {
            try
            {
                IElementHandle oemElement = await new ValidSingleElementSource(
                    new ParentElementSource(blockPart)
                ).Read(string.Intern("p[data-marker='item-oem-number']"));
                string text = await new TextFromWebElement(oemElement).Read();
                return text.Trim();
            }
            catch
            {
                // ignored
            }
        }

        return string.Empty;
    }

    private async Task<IElementHandle[]> MiddleBlockData(IElementHandle item)
    {
        IElementHandle block = await new ParentElementSource(item).Read(
            string.Intern(".iva-item-listMiddleBlock-W7qtU")
        );
        IElementHandle[] blockParts = await new ParentManyElementsSource(block).Read(
            ".iva-item-ivaItemRedesign-QmNXd"
        );
        return blockParts;
    }

    private async Task<LinkedList<string>> ReadImages(IElementHandle item)
    {
        LinkedList<string> images = [];
        IElementHandle slider = await new ValidSingleElementSource(
            new ParentElementSource(item)
        ).Read(string.Intern(".iva-item-slider-BOsti"));
        IElementHandle sliderList = await new ValidSingleElementSource(
            new ParentElementSource(slider)
        ).Read(string.Intern(".photo-slider-list-R0jle"));
        IElementHandle[] imageElements = await new ParentManyElementsSource(sliderList).Read("img");
        foreach (IElementHandle imageElement in imageElements)
        {
            string srcSet = await new AttributeFromWebElement(imageElement, "srcset").Read();
            string[] parts = srcSet.Split(',', StringSplitOptions.TrimEntries);
            string highQualityImage = parts[^1].Split(" ", StringSplitOptions.TrimEntries)[0];
            images.AddFirst(highQualityImage);
        }

        return images;
    }
}
