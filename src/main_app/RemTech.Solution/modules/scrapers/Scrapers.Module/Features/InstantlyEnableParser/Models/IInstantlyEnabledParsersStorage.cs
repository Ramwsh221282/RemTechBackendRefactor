namespace Scrapers.Module.Features.InstantlyEnableParser.Models;

internal interface IInstantlyEnabledParsersStorage
{
    Task<ParserToInstantlyEnable> Fetch(
        string parserName,
        string parserType,
        CancellationToken ct = default
    );
    Task Save(
        string parserName,
        string parserType,
        string state,
        DateTime nextRun,
        DateTime lastRun,
        CancellationToken ct = default
    );
}
