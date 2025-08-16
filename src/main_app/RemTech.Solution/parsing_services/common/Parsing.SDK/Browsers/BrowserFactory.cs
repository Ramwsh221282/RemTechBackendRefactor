using System.Runtime.InteropServices;
using Parsing.SDK.Browsers.BrowserLoadings;
using Parsing.SDK.Browsers.InstantiationModes;
using Parsing.SDK.Browsers.InstantiationOptions;
using Parsing.SDK.Browsers.PathSources;
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

    public static async Task<IScrapingBrowser> Create()
    {
        return _isDevelopment
            ? await ProvideDevelopmentBrowser()
            : await ProvideProductionBrowser();
    }

    public static async Task<IScrapingBrowser> ProvideDevelopmentBrowser()
    {
        IBrowser browser = await new DefaultBrowserInstantiation(
            new HeadlessBrowserInstantiationOptions(),
            new BasicBrowserLoading()
        ).Instantiation();
        return new SinglePagedScrapingBrowser(browser);
    }

    public static async Task<IScrapingBrowser> ProvideProductionBrowser()
    {
        IBrowser browser = await new DefaultBrowserInstantiation(
            new HeadlessWithDirectBrowserPathLaunchOptions(
                new BrowserPathSourceFromEnvironmentVariables("CHROME_BIN")
            ),
            new BasicBrowserLoading()
        ).Instantiation();
        return new SinglePagedScrapingBrowser(browser);
    }
}
