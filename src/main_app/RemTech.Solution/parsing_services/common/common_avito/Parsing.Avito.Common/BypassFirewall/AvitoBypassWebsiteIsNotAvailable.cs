using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Parsing.Avito.Common.BypassFirewall;

public sealed class AvitoBypassWebsiteIsNotAvailable(
    IPage page,
    IAvitoBypassFirewall origin,
    int attempts = 20)
    : IAvitoBypassFirewall
{
    private readonly string _buttonSelector = string.Intern("button");

    private readonly string _titleSelector = string.Intern(".content-wrapper");
    
    public async Task<bool> Read()
    {
        if (!await HasUnavailableTitle())
            return await origin.Read();

        for (int i = 0; i < attempts; i++)
        {
            IElementHandle? button = await new PageElementSource(page).Read(_buttonSelector);
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
        IElementHandle? title = await new PageElementSource(page).Read(_titleSelector);
        if (title == null)
            return false;

        string text = await new TextFromWebElement(title).Read();
        return text.Contains("Сайт временно недоступен", StringComparison.OrdinalIgnoreCase);
    }
}