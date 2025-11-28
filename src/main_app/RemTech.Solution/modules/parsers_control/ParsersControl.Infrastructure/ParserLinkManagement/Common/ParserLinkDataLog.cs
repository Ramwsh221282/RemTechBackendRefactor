using ParsersControl.Core.ParserLinksManagement;

namespace ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkIgnoredListener;

public sealed class ParserLinkDataLog
{
    private readonly Serilog.ILogger _logger;
    private Guid _id = Guid.Empty;
    private string _name = string.Empty;
    private string _url = string.Empty;
    private Guid _parserId = Guid.Empty;
    private bool _isIgnored;
    private void AddId(Guid id) => _id = id;
    private void AddName(string name) => _name = name;
    private void AddUrl(string url) => _url = url;
    private void AddIgnored(bool isIgnored) => _isIgnored = isIgnored;
    private void AddParserId(Guid parserId) => _parserId = parserId;

    public ParserLinkDataLog(ParserLink link, Serilog.ILogger logger)
    {
        _logger = logger;
        link.Write(AddId, AddUrl, AddName, AddIgnored, AddParserId);
    }

    public void Log()
    {
        object[] logProperties = [_id, _name, _url, _isIgnored, _parserId];
        _logger.Information("""
                            Parser link info: 
                            Id: {id}
                            Name: {name}
                            Url: {url}
                            Ignored: {isIgnored}
                            Id: {parserId}
                            """, logProperties);
    }
}