using Npgsql;
using Quartz;
using Scrapers.Module.Features.StartParser.Database;
using Scrapers.Module.Features.StartParser.Models;
using Scrapers.Module.Features.StartParser.RabbitMq;

namespace Scrapers.Module.Features.StartParser.Entrance;

internal sealed class StartingParsersEntrance(
    Serilog.ILogger logger,
    NpgsqlDataSource dataSource,
    IParserStartedPublisher publisher
) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        IParsersToStartStorage storage = new NpgSqlParsersToStartStorage(dataSource);
        IEnumerable<ParserToStart> parsers = await storage.Fetch(DateTime.UtcNow);
        ParserToStart[] array = parsers.ToArray();
        foreach (ParserToStart parser in array)
        {
            StartedParser started = parser.Start();
            await started.Save(storage);
            await started.Publish(publisher);
            logger.Information(
                "Started parser: {Name} {Type} {Domain} links count: {LinksCount}...",
                parser.ParserName,
                parser.ParserType,
                parser.ParserDomain,
                parser.Links.Count
            );
        }
    }
}
