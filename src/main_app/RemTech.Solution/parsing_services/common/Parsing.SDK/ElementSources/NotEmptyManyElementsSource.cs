using PuppeteerSharp;

namespace Parsing.SDK.ElementSources;

public sealed class NotEmptyManyElementsSource : IManyElementsSource
{
    private readonly IManyElementsSource _source;

    public NotEmptyManyElementsSource(IManyElementsSource source) => _source = source;
    
    public async Task<IElementHandle[]> Read(string path)
    {
        IElementHandle[] results =  await _source.Read(path);
        return results.Length == 0
            ? throw new ArgumentNullException($"Element with path: {path} resulted in 0 elements.")
            : results;
    }
}