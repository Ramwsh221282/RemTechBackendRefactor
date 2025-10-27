using Parsing.SDK.ScrapingActions;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;
using RemTech.Core.Shared.Result;

namespace Cleaner.WebApi.CleaningLogic;

internal sealed class DromChallenge(string id, string url, IPage page) : ICheckChallenge
{
    private const string Container = "div[data-ftid='header_breadcrumb']";
    private const string BreadCrumbItem = "div[data-ftid='header_breadcrumb-item']";

    public async Task<Status<string>> ItemIsOutdated()
    {
        await new PageNavigating(page, url).Do();
        await Task.Delay(TimeSpan.FromSeconds(6));
        return await CheckBreadCrumbs();
    }

    private async Task<Status<string>> CheckBreadCrumbs()
    {
        IElementHandle? breadCrumbsContainer = await page.QuerySelectorAsync(Container);
        if (breadCrumbsContainer == null)
            return id;
        IElementHandle[] breadCrumbs = await page.QuerySelectorAllAsync(BreadCrumbItem);
        if (await IsNotRelevant(page))
            return id;
        if (breadCrumbs.Length == 0)
            return id;
        return Error.NotFound("Item is valid");
    }

    private static async Task<bool> IsNotRelevant(IPage page)
    {
        string container = string.Intern(".ftldj60.css-1yado2t");
        IElementHandle? element = await page.QuerySelectorAsync(container);
        if (element == null)
            return false;
        IElementHandle? innerContainer = await element.QuerySelectorAsync(".ftldj61");
        if (innerContainer == null)
            return false;
        IElementHandle? irrelevantContainer = await innerContainer.QuerySelectorAsync(
            ".ebrtcvm0.css-qjhitd.e1u9wqx22"
        );
        if (irrelevantContainer == null)
            return false;
        IElementHandle? irrelevantElement = await irrelevantContainer.QuerySelectorAsync(
            ".css-1jba3gn.edsrp6u3"
        );
        if (irrelevantElement == null)
            return false;
        IElementHandle? irrelevantTextElement = await irrelevantElement.QuerySelectorAsync(
            ".css-1gp719b.edsrp6u2"
        );
        if (irrelevantTextElement == null)
            return false;
        string text = await new TextFromWebElement(irrelevantTextElement).Read();
        return text.Contains("Техника снята с продажи");
    }
}
