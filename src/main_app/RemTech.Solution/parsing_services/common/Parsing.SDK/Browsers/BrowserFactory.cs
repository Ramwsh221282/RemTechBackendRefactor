using Parsing.SDK.Browsers.BrowserLoadings;
using Parsing.SDK.Browsers.InstantiationModes;
using Parsing.SDK.Browsers.InstantiationOptions;
using PuppeteerSharp;

namespace Parsing.SDK.Browsers;

public static class BrowserFactory
{
    private static bool _isDevelopment = false;

    public static void DevelopmentMode() => _isDevelopment = true;

    public static async Task<IScrapingBrowser> Create()
    {
        return await ProvideDevelopmentBrowser();
    }

    public static async Task<IScrapingBrowser> ProvideDevelopmentBrowser()
    {
        IBrowser browser = await new DefaultBrowserInstantiation(
            new HeadlessBrowserInstantiationOptions(),
            new BasicBrowserLoading()
        ).Instantiation();
        return new SinglePagedScrapingBrowser(browser);
    }
}
