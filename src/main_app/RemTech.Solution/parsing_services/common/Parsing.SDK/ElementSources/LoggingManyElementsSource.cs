using PuppeteerSharp;
using Serilog;

namespace Parsing.SDK.ElementSources;

public sealed class LoggingManyElementsSource : IManyElementsSource
{
    private readonly ILogger _log;
    private readonly IManyElementsSource _source;

    public LoggingManyElementsSource(ILogger log, IManyElementsSource source)
    {
        _log = log;
        _source = source;
    }
    
    public Task<IElementHandle[]> Read(string path)
    {
        _log.Information("Reading many elements with path: {0}. Path mode is: {1}.", path, new ElementPathMode(path).Mode());
        return _source.Read(path);
    }
}