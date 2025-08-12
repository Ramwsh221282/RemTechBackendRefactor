using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Parsing.Avito.Common.BypassFirewall;

public sealed class AvitoBypassWebsiteIsNotAvailable(
    IPage page,
    IAvitoBypassFirewall origin,
    int attempts = 20
) : IAvitoBypassFirewall
{
    private const string ButtonSelector = "button";
    private const string TitleSelector = ".content-wrapper";
    private const string NotAvailable = "Сайт временно недоступен";
    private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

    public async Task<bool> Read()
    {
        if (!await HasUnavailableTitle())
            return await origin.Read();

        for (int i = 0; i < attempts; i++)
        {
            IElementHandle? button = await new PageElementSource(page).Read(ButtonSelector);
            if (button != null)
            {
                await button.ClickAsync();
                await Task.Delay(TimeSpan.FromSeconds(5));
            }

            if (!await HasUnavailableTitle())
                break;
        }

        return await origin.Read();
    }

    private async Task<bool> HasUnavailableTitle()
    {
        IElementHandle? title = await new PageElementSource(page).Read(TitleSelector);
        if (title == null)
            return false;
        string text = await new TextFromWebElement(title).Read();
        return text.Contains(NotAvailable, Comparison);
    }
}
