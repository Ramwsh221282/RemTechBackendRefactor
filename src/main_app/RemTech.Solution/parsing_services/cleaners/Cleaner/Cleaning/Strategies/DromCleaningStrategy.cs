using Cleaner.Cleaning.Exceptions;
using Cleaner.Cleaning.RabbitMq;
using Parsing.SDK.ScrapingActions;
using PuppeteerSharp;

namespace Cleaner.Cleaning.Strategies;

internal sealed class DromCleaningStrategy(StartCleaningItemInfo item) : ICleaningStrategy
{
    private const string Container = "div[data-ftid='header_breadcrumb']";
    private const string BreadCrumbItem = "div[data-ftid='header_breadcrumb-item']";

    public async Task Process(IPage page)
    {
        await new PageNavigating(page, item.SourceUrl).Do();
        await Task.Delay(TimeSpan.FromSeconds(6));
        await CheckBreadCrumbs(page);
    }

    private async Task CheckBreadCrumbs(IPage page)
    {
        IElementHandle? breadCrumbsContainer = await page.QuerySelectorAsync(Container);
        if (breadCrumbsContainer == null)
            throw new ItemDoesNotPresentException(item);
        IElementHandle[] breadCrumbs = await page.QuerySelectorAllAsync(BreadCrumbItem);
        if (breadCrumbs.Length == 0)
            throw new ItemDoesNotPresentException(item);
    }
}
