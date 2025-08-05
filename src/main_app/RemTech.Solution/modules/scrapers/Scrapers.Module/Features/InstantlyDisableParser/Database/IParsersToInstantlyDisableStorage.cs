using Scrapers.Module.Features.InstantlyDisableParser.Models;

namespace Scrapers.Module.Features.InstantlyDisableParser.Database;

internal interface IParsersToInstantlyDisableStorage
{
    Task<ParserToInstantlyDisable> Fetch(
        string parserName,
        string parserType,
        CancellationToken ct = default
    );
    Task<InstantlyDisabledParser> Save(
        InstantlyDisabledParser parser,
        CancellationToken ct = default
    );
}
