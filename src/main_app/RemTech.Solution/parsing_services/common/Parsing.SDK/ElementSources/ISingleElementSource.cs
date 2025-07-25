using PuppeteerSharp;

namespace Parsing.SDK.ElementSources;

public interface ISingleElementSource
{
    Task<IElementHandle> Read(string path);
}