using Cleaner.Exceptions;
using Parsing.SDK.ScrapingActions;
using PuppeteerSharp;

namespace Cleaner.Cleaning;

internal sealed class DromChallenge(string id, string url, IPage page) : ICheckChallenge
{
    private const string Container = "div[data-ftid='header_breadcrumb']";
    private const string BreadCrumbItem = "div[data-ftid='header_breadcrumb-item']";

    public async Task Process()
    {
        await new PageNavigating(page, url).Do();
        await Task.Delay(TimeSpan.FromSeconds(6));
        await CheckBreadCrumbs();
    }

    private async Task CheckBreadCrumbs()
    {
        IElementHandle? breadCrumbsContainer = await page.QuerySelectorAsync(Container);
        if (breadCrumbsContainer == null)
            throw new ItemDoesNotPresentException(id);
        IElementHandle[] breadCrumbs = await page.QuerySelectorAllAsync(BreadCrumbItem);
        if (breadCrumbs.Length == 0)
            throw new ItemDoesNotPresentException(id);
    }
}
