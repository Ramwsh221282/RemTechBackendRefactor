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