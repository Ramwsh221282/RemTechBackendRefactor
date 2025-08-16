using Parsing.SDK.Browsers.PageSources;
using PuppeteerSharp;

namespace Parsing.SDK.Browsers;

public sealed class SinglePagedScrapingBrowser(IBrowser browser) : IScrapingBrowser
{
    public void Dispose() => browser.Dispose();

    public async ValueTask DisposeAsync() => await browser.DisposeAsync();

    public async Task<IPage> ProvideDefaultPage()
    {
        IPage page = await browser.NewPageAsync();
        return page;
    }
}
