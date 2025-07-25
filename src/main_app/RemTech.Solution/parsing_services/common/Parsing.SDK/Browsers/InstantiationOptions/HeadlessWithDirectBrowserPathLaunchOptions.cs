using Parsing.SDK.Browsers.PathSources;
using PuppeteerSharp;

namespace Parsing.SDK.Browsers.InstantiationOptions;

public sealed class HeadlessWithDirectBrowserPathLaunchOptions : IScrapingBrowserInstantiationOptions
{
    private readonly IBrowserPathSource _pathSourceSource;

    public HeadlessWithDirectBrowserPathLaunchOptions(IBrowserPathSource pathSourceSource)
    {
        _pathSourceSource = pathSourceSource;
    }
    
    public LaunchOptions Configured()
    {
        string path = _pathSourceSource.Read();
        return new LaunchOptions()
        {
            ExecutablePath = path,
            Headless = true,
            Args = ["--no-sandbox", "--disable-dev-shm-usage", "--start-maximized"],
            IgnoreHTTPSErrors = true,
        };
    }
}