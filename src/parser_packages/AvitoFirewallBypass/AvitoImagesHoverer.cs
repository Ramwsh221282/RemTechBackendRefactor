using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace AvitoFirewallBypass;

public sealed class AvitoImagesHoverer(IPage page)
{
    public async Task Invoke()
    {
        IElementHandle[] catalogueItems = await page.GetElementsRetriable("div[data-marker='item']");
        if (catalogueItems.Length == 0) return;

        foreach (IElementHandle catalogueItem in catalogueItems)
        {
            try
            {
                Maybe<IElementHandle> itemImage = await catalogueItem.GetElementRetriable("div[data-marker='item-image']");
                if (!itemImage.HasValue) continue;
                Maybe<IElementHandle> itemPhoto = await itemImage.Value.GetElementRetriable("div[data-marker='item-photo']");
                if (!itemPhoto.HasValue) continue;
                Maybe<IElementHandle> ulList = await itemPhoto.Value.GetElementRetriable("ul");
                if (!ulList.HasValue) continue;
                IElementHandle[] liElements = await ulList.Value.GetElements("li");
                foreach (IElementHandle li in liElements)
                {
                    await li.HoverAsync();
                }
            }
            catch { }
        }
    }
}