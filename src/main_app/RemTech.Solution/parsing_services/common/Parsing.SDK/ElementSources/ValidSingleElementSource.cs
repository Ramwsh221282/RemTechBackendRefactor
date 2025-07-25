using PuppeteerSharp;

namespace Parsing.SDK.ElementSources;

public sealed class ValidSingleElementSource : ISingleElementSource
{
    private readonly ISingleElementSource _source;

    public ValidSingleElementSource(ISingleElementSource source) =>
        _source = source;
    
    public async Task<IElementHandle> Read(string path)
    {
        IElementHandle? result = await _source.Read(path);
        return result ?? throw new ArgumentNullException($"Element with path: {path} resulted in null element.");
    }
}