using Scrapers.Module.Features.StartParser.Models;

namespace Scrapers.Module.Features.StartParser.Database;

internal interface IParsersToStartStorage
{
    Task<IEnumerable<ParserToStart>> Fetch(CancellationToken ct = default);
    Task<StartedParser> Save(StartedParser parser, CancellationToken ct = default);
}
