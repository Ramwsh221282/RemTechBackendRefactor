using Parsing.SDK.ScrapingActions;
using PuppeteerSharp;

namespace Drom.Parsing.Vehicles.Parsing.Models;

public sealed class DromCatalogueCarNavigation(string url)
{
    public async Task Navigate(IPage page)
    {
        await new PageNavigating(page, url).Do();
        await Task.Delay(TimeSpan.FromSeconds(6));
    }
}
