using AvitoSparesParser.Common;
using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace AvitoSparesParser.Commands.PrepareAvitoPage;

public sealed class PrepareAvitoPageCommand(
    Func<Task<IPage>> pageSource,
    AvitoBypassFactory bypassFactory,
    params Func<IPage, Task>[] afterMainActions
    ) : IPrepareAvitoPageCommand
{
    public async Task Prepare(Func<string> urlSource)
    {
        string url = urlSource();
        IPage page = await pageSource();
        await page.PerformQuickNavigation(url, timeout: 2000);
        
        if (!await bypassFactory.Create(page).Bypass()) 
            throw new InvalidOperationException("Bypass failed.");
        
        await afterMainActions.InvokeForEach(async act => await act(page));
    }
}