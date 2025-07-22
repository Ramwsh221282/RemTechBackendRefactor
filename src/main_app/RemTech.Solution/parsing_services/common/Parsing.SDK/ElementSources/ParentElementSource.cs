using PuppeteerSharp;

namespace Parsing.SDK.ElementSources;

public sealed class ParentElementSource : ISingleElementSource
{
    private readonly IElementHandle _element;

    public ParentElementSource(IElementHandle element) => _element = element;
    
    public Task<IElementHandle> Read(string path) => _element.QuerySelectorAsync(path);
}