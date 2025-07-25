using PuppeteerSharp;

namespace Parsing.SDK.ScrapingArtifacts;

public sealed class TextFromWebElement : IScrapingArtifact<string>
{
    private readonly IElementHandle? _elementHandle;

    public TextFromWebElement(IElementHandle? elementHandle) =>
        _elementHandle = elementHandle;
    
    public Task<string> Read() =>
        _elementHandle == null ?
            Task.FromResult(string.Empty) 
            : _elementHandle.EvaluateFunctionAsync<string>(string.Intern("el => el.textContent"));
}

public sealed class InnerTextFromWebElement : IScrapingArtifact<string>
{
    private readonly IElementHandle? _element;

    public InnerTextFromWebElement(IElementHandle? element)
    {
        _element = element;
    }
    
    public async Task<string> Read()
    {
        return _element == null ?
            string.Empty
            : await _element.EvaluateFunctionAsync<string>(string.Intern("el => el.innerText"));
    }
}