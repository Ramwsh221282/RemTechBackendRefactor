using Parsing.SDK.Logging;

namespace Parsing.SDK.ScrapingActions;

public sealed class LoggingPageNavigating(
    IParsingLog log, 
    string url, 
    IPageNavigating origin) : IPageNavigating
{
    public async Task Do()
    {
        log.Info("Navigating page: {0}.", url);
        await origin.Do();
        log.Info("Navigated on: {0}.", url);
    }
}