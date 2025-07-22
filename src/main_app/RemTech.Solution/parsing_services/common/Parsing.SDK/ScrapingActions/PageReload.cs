using PuppeteerSharp;

namespace Parsing.SDK.ScrapingActions;

public sealed  class PageReload : IPageAction
{
    private readonly IPage _page;

    public PageReload(IPage page) => _page = page;
    
    public async  Task Do()
    {
        NavigationOptions opts = new()
        {
            Timeout = 0,
            WaitUntil = [WaitUntilNavigation.DOMContentLoaded]
        };

        try
        {
            await _page.ReloadAsync(opts);
        }
        catch
        {
            // ignored
        }
    }
}