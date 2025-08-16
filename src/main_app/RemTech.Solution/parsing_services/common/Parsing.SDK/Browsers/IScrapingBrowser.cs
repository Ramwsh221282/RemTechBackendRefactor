using PuppeteerSharp;

namespace Parsing.SDK.Browsers;

public interface IScrapingBrowser : IDisposable, IAsyncDisposable
{
    Task<IPage> ProvideDefaultPage();
}
