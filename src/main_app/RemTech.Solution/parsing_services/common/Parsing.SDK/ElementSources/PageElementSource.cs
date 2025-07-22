using PuppeteerSharp;

namespace Parsing.SDK.ElementSources;

public sealed class PageElementSource : ISingleElementSource
{
    private readonly IPage _page;

    public PageElementSource(IPage page) =>
        _page = page;

    public Task<IElementHandle> Read(string path) =>
        _page.QuerySelectorAsync(path);
}