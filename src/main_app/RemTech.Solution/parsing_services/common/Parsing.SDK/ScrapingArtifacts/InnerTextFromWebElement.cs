using PuppeteerSharp;

namespace Parsing.SDK.ScrapingArtifacts;

public sealed class InnerTextFromWebElement(IElementHandle? element) : IScrapingArtifact<string>
{
    private const string Script = "el => el.textContent";

    public async Task<string> Read()
    {
        return element == null ? string.Empty : await element.EvaluateFunctionAsync<string>(Script);
    }
}
