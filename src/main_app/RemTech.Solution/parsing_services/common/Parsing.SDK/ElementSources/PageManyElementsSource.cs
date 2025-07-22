using PuppeteerSharp;

namespace Parsing.SDK.ElementSources;

public sealed class PageManyElementsSource : IManyElementsSource
{
    private readonly IPage _page;

    public PageManyElementsSource(IPage page) => _page = page;
    
    public Task<IElementHandle[]> Read(string path) => _page.QuerySelectorAllAsync(path);
}