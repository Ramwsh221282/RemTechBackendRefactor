using Parsing.SDK.Browsers.PageSources;
using PuppeteerSharp;

namespace Parsing.SDK.Browsers;

public sealed class SinglePagedScrapingBrowser : IScrapingBrowser
{
    private readonly IBrowser _browser;
    private readonly string _url;

    public SinglePagedScrapingBrowser(IBrowser browser, string url)
    {
        _browser = browser;
        _url = url;
    }
    
    public void Dispose()
    {
        _browser.Dispose();
    }

    public async  ValueTask DisposeAsync()
    {
        await _browser.DisposeAsync();
    }

    public async Task<IBrowserPagesSource> AccessPages()
    {
        return await new NoPageSource().Single(_browser, _url);
    }
}