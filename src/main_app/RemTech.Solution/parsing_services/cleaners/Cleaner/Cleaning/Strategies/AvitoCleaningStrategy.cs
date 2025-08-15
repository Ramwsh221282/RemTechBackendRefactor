using Cleaner.Cleaning.Exceptions;
using Cleaner.Cleaning.RabbitMq;
using Parsing.Avito.Common.BypassFirewall;
using Parsing.SDK.ScrapingActions;
using PuppeteerSharp;
using RemTech.Core.Shared.Decorating;

namespace Cleaner.Cleaning.Strategies;

internal sealed class AvitoCleaningStrategy(StartCleaningItemInfo item, Serilog.ILogger logger)
    : ICleaningStrategy
{
    private const string ContainerSelector = "#bx_item-breadcrumbs";
    private const string BreadCrumbsSelector = "span[itemprop='itemListElement']";

    public async Task Process(IPage page)
    {
        IAvitoBypassFirewall bypass = new AvitoBypassFirewall(page)
            .WrapBy(p => new AvitoBypassFirewallExceptionSupressing(p))
            .WrapBy(p => new AvitoBypassFirewallLazy(page, p))
            .WrapBy(p => new AvitoBypassRepetetive(page, p))
            .WrapBy(p => new AvitoBypassWebsiteIsNotAvailable(page, p))
            .WrapBy(p => new AvitoBypassFirewallLogging(logger, p));
        await new PageNavigating(page, item.SourceUrl).Do();
        if (!await bypass.Read())
            return;
        await CheckBreadCrumbs(page);
    }

    private async Task CheckBreadCrumbs(IPage page)
    {
        IElementHandle? breadCrumbsContainer = await page.QuerySelectorAsync(ContainerSelector);
        if (breadCrumbsContainer == null)
            throw new ItemDoesNotPresentException(item);
        IElementHandle[] breadCrumbs = await breadCrumbsContainer.QuerySelectorAllAsync(
            BreadCrumbsSelector
        );
        if (breadCrumbs.Length == 0)
            throw new ItemDoesNotPresentException(item);
    }
}
