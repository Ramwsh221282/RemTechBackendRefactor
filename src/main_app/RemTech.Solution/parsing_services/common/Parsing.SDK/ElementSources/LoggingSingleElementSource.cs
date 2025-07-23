using PuppeteerSharp;
using RemTech.Logging.Library;

namespace Parsing.SDK.ElementSources;

public sealed class LoggingSingleElementSource : ISingleElementSource
{
    private readonly ICustomLogger _log;
    private readonly ISingleElementSource _source;

    public LoggingSingleElementSource(ICustomLogger log, ISingleElementSource source)
    {
        _log = log;
        _source = source;
    }
    
    public Task<IElementHandle> Read(string path)
    {
        _log.Info("Reading element with path: {0}. Path mode is: {1}.", path, new ElementPathMode(path).Mode());
        return _source.Read(path);
    }
}