using Npgsql;
using Quartz;
using Scrapers.Module.Domain.JournalsContext.Cache;
using Scrapers.Module.Domain.JournalsContext.Features.CreateScraperJournal;
using Scrapers.Module.Features.StartParser.Database;
using Scrapers.Module.Features.StartParser.Models;
using Scrapers.Module.Features.StartParser.RabbitMq;
using Scrapers.Module.ParserStateCache;
using StackExchange.Redis;

namespace Scrapers.Module.Features.StartParser.Entrance;

public sealed class StartingParsersEntrance(
    Serilog.ILogger logger,
    NpgsqlDataSource dataSource,
    IParserStartedPublisher publisher,
    ConnectionMultiplexer multiplexer
) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        ParserStateCachedStorage cache = new ParserStateCachedStorage(multiplexer);
        ActiveScraperJournalsCache journalsCache = new ActiveScraperJournalsCache(multiplexer);
        IParsersToStartStorage storage = new NpgSqlParsersToStartStorage(dataSource);
        IEnumerable<ParserToStart> parsers = await storage.Fetch(DateTime.UtcNow);
        ParserToStart[] array = parsers.ToArray();
        foreach (ParserToStart parser in array)
        {
            StartedParser started = parser.Start();
            await started.Save(storage);
            await cache.UpdateState(started.ParserName, started.ParserType, started.ParserState);
            await new CreateScraperJournalHandler(dataSource, logger, journalsCache).Handle(
                started.CreateJournalCommand()
            );
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
