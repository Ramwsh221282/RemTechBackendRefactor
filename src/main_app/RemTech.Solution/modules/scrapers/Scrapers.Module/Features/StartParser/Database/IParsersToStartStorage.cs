using Scrapers.Module.Features.StartParser.Models;

namespace Scrapers.Module.Features.StartParser.Database;

public interface IParsersToStartStorage
{
    Task<IEnumerable<ParserToStart>> Fetch(DateTime nextRun, CancellationToken ct = default);
    Task<StartedParser> Save(StartedParser parser, CancellationToken ct = default);
}
