using Cleaner.WebApi.Models;
using PuppeteerSharp;

namespace Cleaner.WebApi.CleaningLogic;

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

    public SuspiciousItem(CleanerProcessingItem item)
        : this(item.Id, item.SourceUrl, item.Domain) { }
}
