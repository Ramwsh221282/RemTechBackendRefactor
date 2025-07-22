using Parsing.SDK.Browsers.BrowserLoadings;
using Parsing.SDK.Browsers.InstantiationOptions;
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerSharp;

namespace Parsing.SDK.Browsers.InstantiationModes;

public sealed class DefaultBrowserInstantiation : IScrapingBrowserInstantiation
{
    private readonly IScrapingBrowserInstantiationOptions _options;
    private readonly IBrowserLoading _loading;
    
    public DefaultBrowserInstantiation(
        IScrapingBrowserInstantiationOptions options,
        IBrowserLoading loading)
    {
        _options = options;
        _loading = loading;
    }
    
    public async Task<IBrowser> Instantiation()
    {
        await _loading.LoadBrowser();
        PuppeteerExtra extra = new();
        StealthPlugin plugin = new();
        return await extra.Use(plugin).LaunchAsync(_options.Configured());
    }
}