using Parsing.SDK.Browsers.PageSources;

namespace Parsing.SDK.Browsers;

public interface IScrapingBrowser : IDisposable, IAsyncDisposable
{
    Task<IBrowserPagesSource> AccessPages();
}