using PuppeteerSharp;

namespace Parsing.SDK.Browsers.InstantiationOptions;

public sealed class HeadlessBrowserInstantiationOptions : IScrapingBrowserInstantiationOptions
{
    public LaunchOptions Configured()
    {
        return new LaunchOptions()
        {
            Headless = true,
            Args = ["--no-sandbox", "--disable-dev-shm-usage", "--start-maximized"],
            IgnoreHTTPSErrors = true,
            DefaultViewport = null!
        };
    }
}