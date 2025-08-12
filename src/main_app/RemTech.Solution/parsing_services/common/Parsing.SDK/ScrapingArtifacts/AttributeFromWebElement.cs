using PuppeteerSharp;

namespace Parsing.SDK.ScrapingArtifacts;

public sealed class AttributeFromWebElement(IElementHandle? elementHandle, string attributeName)
    : IScrapingArtifact<string>
{
    public Task<string> Read()
    {
        if (string.IsNullOrEmpty(attributeName))
            throw new ArgumentException(
                $"Attribute name in {nameof(AttributeFromWebElement)} was not provided."
            );
        return elementHandle == null
            ? Task.FromResult(string.Empty)
            : elementHandle.EvaluateFunctionAsync<string>(
                string.Intern($"el => el.getAttribute('{attributeName}')")
            );
    }
}
