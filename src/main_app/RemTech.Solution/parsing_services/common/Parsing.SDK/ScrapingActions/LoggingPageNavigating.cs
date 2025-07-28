using Serilog;

namespace Parsing.SDK.ScrapingActions;

public sealed class LoggingPageNavigating(
    ILogger log, 
    string url, 
    IPageNavigating origin) : IPageNavigating
{
    public async Task Do()
    {
        log.Information("Navigating page: {0}.", url);
        await origin.Do();
        log.Information("Navigated on: {0}.", url);
    }
}