using PuppeteerSharp;

namespace Cleaner.Cleaning;

internal sealed class SuspiciousItem(string id, string url, string domain)
{
    public ICheckChallenge SpecifyChallenge(IPage page, Serilog.ILogger logger)
    {
        return domain switch
        {
            "avito.ru" => new AvitoChallenge(id, url, page, logger),
            "drom.ru" => new DromChallenge(id, url, page),
            _ => throw new ArgumentException($"{domain} unsupported suspicious item domain."),
        };
    }
}
