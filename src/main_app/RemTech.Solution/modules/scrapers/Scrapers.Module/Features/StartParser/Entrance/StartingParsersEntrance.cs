using Npgsql;
using Quartz;
using Scrapers.Module.Features.StartParser.Database;
using Scrapers.Module.Features.StartParser.Models;

namespace Scrapers.Module.Features.StartParser.Entrance;

internal sealed class StartingParsersEntrance(Serilog.ILogger logger, NpgsqlDataSource dataSource)
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        IParsersToStartStorage storage = new NpgSqlParsersToStartStorage(dataSource);
        IEnumerable<ParserToStart> parsers = await storage.Fetch();
        ParserToStart[] array = parsers.ToArray();
        logger.Information("{Count} parsers to start.", array.Length);
        foreach (ParserToStart parser in array)
        {
            logger.Information(
                "Starting parser: {Name} {Type} {Domain} links count: {LinksCount}...",
                parser.ParserName,
                parser.ParserType,
                parser.ParserDomain,
                parser.Links.Count
            );
        }
    }
}
