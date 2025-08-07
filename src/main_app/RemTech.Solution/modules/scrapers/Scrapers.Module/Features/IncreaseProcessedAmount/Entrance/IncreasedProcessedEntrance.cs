using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Scrapers.Module.Features.IncreaseProcessedAmount.Database;
using Scrapers.Module.Features.IncreaseProcessedAmount.MessageBus;
using Scrapers.Module.Features.IncreaseProcessedAmount.Models;

namespace Scrapers.Module.Features.IncreaseProcessedAmount.Entrance;

internal sealed class IncreasedProcessedEntrance(
    Channel<IncreaseProcessedMessage> channel,
    NpgsqlDataSource dataSource,
    Serilog.ILogger logger
) : BackgroundService
{
    private readonly ChannelReader<IncreaseProcessedMessage> _reader = channel.Reader;

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        logger.Information("{Entrance} is starting...", nameof(IncreasedProcessedEntrance));
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.Information("{Entrance} started.", nameof(IncreasedProcessedEntrance));
        while (!stoppingToken.IsCancellationRequested)
        {
            await _reader.WaitToReadAsync(stoppingToken);
            while (_reader.TryRead(out var message))
            {
                IParserToIncreaseStorage storage = new NpgSqlParserToIncreaseStorage(dataSource);
                ParserToIncreaseProcessed parser = await storage.Fetch(
                    message.ParserName,
                    message.ParserType,
                    message.LinkName,
                    stoppingToken
                );
                ParserWithIncreasedProcessed increased = parser.Increase();
                try
                {
                    ParserWithIncreasedProcessed saved = await storage.Save(
                        increased,
                        stoppingToken
                    );
                    logger.Information(
                        "Parser increased processed: {Name} {Type} {Count}",
                        saved.ParserName,
                        saved.ParserType,
                        saved.ParserProcessed
                    );
                    logger.Information(
                        "Link increased processed: {Name} {Count}",
                        saved.ParserLinkName,
                        saved.LinkProcessed
                    );
                }
                catch (Exception ex)
                {
                    logger.Fatal(
                        "{Entrance} Fatal. {Ex}.",
                        nameof(IncreasedProcessedEntrance),
                        ex.Message
                    );
                }
            }
        }
    }
}
