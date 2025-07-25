using RemTech.Logging.Library;

namespace Parsing.SDK.ScrapingActions;

public sealed class LoggingPageNavigating(
    ICustomLogger log, 
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