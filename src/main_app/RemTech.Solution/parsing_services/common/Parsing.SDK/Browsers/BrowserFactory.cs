using Parsing.SDK.Browsers.BrowserLoadings;
using Parsing.SDK.Browsers.InstantiationModes;
using Parsing.SDK.Browsers.InstantiationOptions;
using PuppeteerSharp;

namespace Parsing.SDK.Browsers;

public static class BrowserFactory
{
    private static bool _isDevelopment;

    public static void DevelopmentMode()
    {
        _isDevelopment = true;
        Console.WriteLine("Development mode set.");
    }

    public static void ProductionMode()
    {
        _isDevelopment = false;
        Console.WriteLine("Producation mode set.");
    }

    public static async Task<IScrapingBrowser> ProvideBrowser()
    {
        IBrowser browser = await new DefaultBrowserInstantiation(
            new HeadlessBrowserInstantiationOptions(),
            new BasicBrowserLoading()
        ).Instantiation();
        return new SinglePagedScrapingBrowser(browser);
    }

    public static async Task<IScrapingBrowser> ProvideNonHeadlessBrowser()
    {
        IBrowser browser = await new DefaultBrowserInstantiation(
            new NonHeadlessBrowserInstantiationOptions(),
            new BasicBrowserLoading()
        ).Instantiation();
        return new SinglePagedScrapingBrowser(browser);
    }
}
