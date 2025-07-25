using PuppeteerSharp;

namespace Parsing.SDK.Browsers.InstantiationOptions;

public sealed class NonHeadlessBrowserInstantiationOptions : IScrapingBrowserInstantiationOptions
{
    public LaunchOptions Configured()
    {
        return new LaunchOptions()
        {
            Headless = false,
            Args = ["--no-sandbox", "--disable-dev-shm-usage", "--start-maximized"],
            IgnoreHTTPSErrors = true,
            DefaultViewport = null!
        };
    }
}