using PuppeteerSharp;

namespace Parsing.SDK.Browsers.PageSources;

public sealed class SinglePageSource : IBrowserPagesSource
{
    private readonly IPage _page;
    private readonly string _initialUrl;

    public SinglePageSource(IPage page, string initialUrl)
    {
        _page = page;
        _initialUrl = initialUrl;
    }

    public void Dispose() => _page.Dispose();

    public ValueTask DisposeAsync() => _page.DisposeAsync();

    public IEnumerable<IPage> Iterate()
    {
        yield return _page;
    }
}