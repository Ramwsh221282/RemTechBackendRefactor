using PuppeteerSharp;

namespace Parsing.SDK.Browsers.BrowserLoadings;

public sealed class BasicBrowserLoading : IBrowserLoading
{
    public async Task LoadBrowser()
    {
        using BrowserFetcher fetcher = new();
        await fetcher.DownloadAsync();
    }
}