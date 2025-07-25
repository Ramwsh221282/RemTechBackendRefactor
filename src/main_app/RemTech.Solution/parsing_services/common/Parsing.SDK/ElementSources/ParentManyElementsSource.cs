using PuppeteerSharp;

namespace Parsing.SDK.ElementSources;

public sealed class ParentManyElementsSource : IManyElementsSource
{
    private readonly IElementHandle _element;

    public ParentManyElementsSource(IElementHandle element) => _element = element;
    
    public Task<IElementHandle[]> Read(string path) =>
        _element.QuerySelectorAllAsync(path);
}