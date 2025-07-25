using PuppeteerSharp;

namespace Parsing.SDK.Browsers.PageSources;

public interface IBrowserPagesSource : IDisposable, IAsyncDisposable
{
    IEnumerable<IPage> Iterate();
}