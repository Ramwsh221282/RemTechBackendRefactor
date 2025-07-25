using PuppeteerSharp;

namespace Parsing.SDK.ScrapingArtifacts;

public sealed class AttributeFromWebElement : IScrapingArtifact<string>
{
    private readonly IElementHandle? _elementHandle;
    private readonly string _attributeName;

    public AttributeFromWebElement(IElementHandle? elementHandle, string attributeName)
    {
        _elementHandle = elementHandle;
        _attributeName = attributeName;
    }
    
    public Task<string> Read()
    {
        if (string.IsNullOrEmpty(_attributeName))
            throw new ArgumentException($"Attribute name in {nameof(AttributeFromWebElement)} was not provided.");
        return _elementHandle == null ?
            Task.FromResult(string.Empty) :
            _elementHandle.EvaluateFunctionAsync<string>(string.Intern($"el => el.getAttribute('{_attributeName}')"));
    }
}