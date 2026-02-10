using Microsoft.Extensions.Options;

namespace ParsingSDK.Parsing;

public sealed class BrowserManagerProvider
{
    private readonly IOptions<ScrapingBrowserOptions> _options;
    private readonly Serilog.ILogger _logger;

    public BrowserManagerProvider(IOptions<ScrapingBrowserOptions> options, Serilog.ILogger logger)
    {
        _options = options;
        _logger = logger;
    }

    public BrowserManager Provide()
    {
        return new BrowserManager(_options, _logger);
    }
}