using Cleaner.Exceptions;
using Parsing.Avito.Common.BypassFirewall;
using Parsing.SDK.ScrapingActions;
using PuppeteerSharp;
using RemTech.Core.Shared.Decorating;

namespace Cleaner.Cleaning;

internal sealed class AvitoChallenge(string id, string url, IPage page, Serilog.ILogger logger)
    : ICheckChallenge
{
    private const string ContainerSelector = "#bx_item-breadcrumbs";
    private const string BreadCrumbsSelector = "span[itemprop='itemListElement']";

    public async Task Process()
    {
        IAvitoBypassFirewall bypass = FormBypass();
        await new PageNavigating(page, url).Do();
        if (!await bypass.Read())
            return;
        await CheckBreadCrumbs();
    }

    private IAvitoBypassFirewall FormBypass()
    {
        IAvitoBypassFirewall bypass = new AvitoBypassFirewall(page)
            .WrapBy(p => new AvitoBypassFirewallExceptionSupressing(p))
            .WrapBy(p => new AvitoBypassFirewallLazy(page, p))
            .WrapBy(p => new AvitoBypassRepetetive(page, p))
            .WrapBy(p => new AvitoBypassWebsiteIsNotAvailable(page, p))
            .WrapBy(p => new AvitoBypassFirewallLogging(logger, p));
        return bypass;
    }

    private async Task CheckBreadCrumbs()
    {
        IElementHandle? breadCrumbsContainer = await page.QuerySelectorAsync(ContainerSelector);
        if (breadCrumbsContainer == null)
            throw new ItemDoesNotPresentException(id);
        IElementHandle[] breadCrumbs = await breadCrumbsContainer.QuerySelectorAllAsync(
            BreadCrumbsSelector
        );
        if (breadCrumbs.Length == 0)
            throw new ItemDoesNotPresentException(id);
    }
}
