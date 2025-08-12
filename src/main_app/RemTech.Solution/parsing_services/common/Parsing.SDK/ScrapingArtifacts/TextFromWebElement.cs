using PuppeteerSharp;

namespace Parsing.SDK.ScrapingArtifacts;

public sealed class TextFromWebElement(IElementHandle? elementHandle) : IScrapingArtifact<string>
{
    private const string Script = "el => el.textContent";

    public Task<string> Read() =>
        elementHandle == null
            ? Task.FromResult(string.Empty)
            : elementHandle.EvaluateFunctionAsync<string>(Script);
}
