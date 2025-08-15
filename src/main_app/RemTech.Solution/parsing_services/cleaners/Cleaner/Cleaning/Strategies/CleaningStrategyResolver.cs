using System.Diagnostics;
using Cleaner.Cleaning.RabbitMq;

namespace Cleaner.Cleaning.Strategies;

internal sealed class CleaningStrategyResolver(Serilog.ILogger logger)
{
    private readonly Serilog.ILogger _logger = logger;

    public ICleaningStrategy ResolveByDomain(StartCleaningItemInfo item)
    {
        return item.Domain switch
        {
            "avito.ru" => new AvitoCleaningStrategy(item, logger),
            "drom.ru" => new DromCleaningStrategy(item),
            _ => throw new UnreachableException($"{item.Domain} is not supported."),
        };
    }
}
