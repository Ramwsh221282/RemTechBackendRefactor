using PuppeteerSharp;

namespace Parsing.SDK.ScrapingActions;

public sealed class PageNavigating(IPage page, string url) : IPageNavigating
{
    public async  Task Do()
    {
        try
        {
            NavigationOptions opts = new()
            {
                Timeout = 0,
                WaitUntil = [WaitUntilNavigation.DOMContentLoaded]
            };
            await page.GoToAsync(url, opts);
        }
        catch
        {
            Console.WriteLine("Exception as instantiating single page source with DOM Content load. Ignored.");
        }
    }
}