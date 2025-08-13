using System.Text;
using Parsing.Avito.Common.Images;
using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;
using RemTech.Core.Shared.Primitives;

namespace Avito.Vehicles.Service.VehiclesParsing.CatalogueItems;

public sealed class ExtractingAvitoCatalogueItemsSource(
    IPage page,
    IAvitoCatalogueItemsSource origin
) : IAvitoCatalogueItemsSource
{
    private readonly string _containerSelector = string.Intern("#bx_serp-item-list");
    private readonly string _itemSelector = string.Intern("div[data-marker='item']");

    public async Task<CatalogueItemsList> Read()
    {
        CatalogueItemsList fromOrigin = await origin.Read();
        IElementHandle container = await new ValidSingleElementSource(
            new PageElementSource(page)
        ).Read(_containerSelector);
        IElementHandle[] items = await new ParentManyElementsSource(container).Read(_itemSelector);
        foreach (var item in items)
        {
            NotEmptyString url = await ExtractUrl(item);
            NotEmptyString[] photos = await ExtractImages(item);
            CatalogueItem entry = new CatalogueItem(url, photos);
            fromOrigin = fromOrigin.With(entry);
        }

        return fromOrigin;
    }

    private async Task<NotEmptyString> ExtractUrl(IElementHandle item)
    {
        IElementHandle title = await new ValidSingleElementSource(
            new ParentElementSource(item)
        ).Read(string.Intern(".iva-item-listTopBlock-n6Rva"));
        IElementHandle url = await new ValidSingleElementSource(
            new ParentElementSource(title)
        ).Read(string.Intern("a[itemprop='url']"));
        string href = await new AttributeFromWebElement(url, "href").Read();
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Intern("https://www.avito.ru"));
        sb.Append(href);
        return new NotEmptyString(sb.ToString());
    }

    private static async Task<NotEmptyString[]> ExtractImages(IElementHandle item)
    {
        IEnumerable<string> images = await new AvitoImagesProvider(item).GetImages();
        return images.Select(x => new NotEmptyString(x)).ToArray();
    }
}
