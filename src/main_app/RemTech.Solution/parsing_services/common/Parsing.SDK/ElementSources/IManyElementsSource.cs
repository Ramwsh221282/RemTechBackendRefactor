using PuppeteerSharp;

namespace Parsing.SDK.ElementSources;

public interface IManyElementsSource
{
    Task<IElementHandle[]> Read(string path);
}