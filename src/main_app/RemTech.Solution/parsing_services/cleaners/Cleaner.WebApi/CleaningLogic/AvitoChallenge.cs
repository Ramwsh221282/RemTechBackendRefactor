using Parsing.Avito.Common.BypassFirewall;
using Parsing.SDK.ScrapingActions;
using PuppeteerSharp;
using RemTech.Core.Shared.Decorating;
using RemTech.Core.Shared.Result;

namespace Cleaner.WebApi.CleaningLogic;

internal sealed class AvitoChallenge(string id, string url, IPage page, Serilog.ILogger logger)
    : ICheckChallenge
{
    private const string ContainerSelector = "#bx_item-breadcrumbs";
    private const string BreadCrumbsSelector = "span[itemprop='itemListElement']";

    public async Task<Status<string>> ItemIsOutdated()
    {
        IAvitoBypassFirewall bypass = FormBypass();
        await new PageNavigating(page, url).Do();
        return !await bypass.Read() ? id : await CheckBreadCrumbs();
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

    private async Task<Status<string>> CheckBreadCrumbs()
    {
        var breadCrumbsContainer = await page.QuerySelectorAsync(ContainerSelector);
        if (breadCrumbsContainer == null)
            return id;

        var breadCrumbs = await breadCrumbsContainer.QuerySelectorAllAsync(BreadCrumbsSelector);
        return breadCrumbs.Length == 0 ? id : Error.NotFound("Item valid.");
    }
}
