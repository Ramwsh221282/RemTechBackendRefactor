using Parsing.Avito.Common.Images;
using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Avito.Parsing.Spares.Parsing;

public sealed class AvitoSparesCollection : IAvitoSparesCollection
{
    private readonly IPage _page;
    private const string ItemListContainer = "#bx_serp-item-list";
    private const string ItemListItem = "div[data-marker='item']";

    public AvitoSparesCollection(IPage page) => _page = page;

    public async Task<IEnumerable<AvitoSpare>> Read()
    {
        Console.WriteLine("Hovering images.");
        await new AvitoImagesHoverer(_page).HoverImages();
        Console.WriteLine("Images hovered.");
        Console.WriteLine("Extracting items container.");
        IElementHandle listContainer = await new ValidSingleElementSource(
            new PageElementSource(_page)
        ).Read(ItemListContainer);
        Console.WriteLine("Container extracted.");
        IElementHandle[] items = await new ParentManyElementsSource(listContainer).Read(
            ItemListItem
        );
        Console.WriteLine("Items extracted");
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

    private const string TitleContainer = ".iva-item-title-KE8A9";

    private async Task<string> ReadTitle(IElementHandle item)
    {
        IElementHandle titleElement = await new ValidSingleElementSource(
            new ParentElementSource(item)
        ).Read(TitleContainer);
        return await new TextFromWebElement(titleElement).Read();
    }

    private const string SourceUrlContainer = ".iva-item-title-KE8A9";
    private const string SourceUrlTitle = "a[data-marker='item-title'";
    private const string HrefAttribute = "href";

    private async Task<string> ReadSourceUrl(IElementHandle item)
    {
        IElementHandle titleContainer = await new ValidSingleElementSource(
            new ParentElementSource(item)
        ).Read(SourceUrlContainer);
        IElementHandle titleElement = await new ValidSingleElementSource(
            new ParentElementSource(titleContainer)
        ).Read(SourceUrlTitle);
        string href = await new AttributeFromWebElement(titleElement, HrefAttribute).Read();
        return href;
    }

    private const string PriceContainer = "p[data-marker='item-price'";
    private const string PriceMeta = "meta[itemprop='price']";
    private const string PriceNds = "НДС";
    private const string PriceAttribute = "Content";

    private async Task<(long, bool)> ReadItemPrice(IElementHandle item)
    {
        IElementHandle priceContainer = await new ValidSingleElementSource(
            new ParentElementSource(item)
        ).Read(PriceContainer);
        IElementHandle priceMeta = await new ValidSingleElementSource(
            new ParentElementSource(priceContainer)
        ).Read(PriceMeta);
        string value = await new AttributeFromWebElement(priceMeta, PriceAttribute).Read();
        string text = await new TextFromWebElement(priceContainer).Read();
        return text.Contains(PriceNds, StringComparison.OrdinalIgnoreCase)
            ? (long.Parse(value), true)
            : (long.Parse(value), false);
    }

    private const string GeoContainer = ".geo-root-BBVai";

    private async Task<string> ReadGeoInfo(IElementHandle[] blockParts)
    {
        foreach (IElementHandle blockPart in blockParts)
        {
            try
            {
                IElementHandle geoRoot = await new ValidSingleElementSource(
                    new ParentElementSource(blockPart)
                ).Read(GeoContainer);
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

    private const string RelatedBrandContainer = ".iva-item-text-PvwMY";

    private async Task<string> ReadRelatedBrand(IElementHandle[] blockParts)
    {
        foreach (IElementHandle blockPart in blockParts)
        {
            try
            {
                IElementHandle relatedBrandElement = await new ValidSingleElementSource(
                    new ParentElementSource(blockPart)
                ).Read(RelatedBrandContainer);
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

    private const string OemContainer = "p[data-marker='item-oem-number']";

    private async Task<string> ReadOem(IElementHandle[] blockParts)
    {
        foreach (IElementHandle blockPart in blockParts)
        {
            try
            {
                IElementHandle oemElement = await new ValidSingleElementSource(
                    new ParentElementSource(blockPart)
                ).Read(OemContainer);
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

    private const string MiddleBlockContainer = ".iva-item-listMiddleBlock-W7qtU";
    private const string MiddleBlockBlocks = ".iva-item-ivaItemRedesign-QmNXd";

    private async Task<IElementHandle[]> MiddleBlockData(IElementHandle item)
    {
        IElementHandle block = await new ParentElementSource(item).Read(MiddleBlockContainer);
        IElementHandle[] blockParts = await new ParentManyElementsSource(block).Read(
            MiddleBlockBlocks
        );
        return blockParts;
    }

    private async Task<IEnumerable<string>> ReadImages(IElementHandle item)
    {
        IEnumerable<string> images = await new AvitoImagesProvider(item).GetImages();
        return images;
    }
}
