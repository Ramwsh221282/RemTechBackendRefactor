using System.Text.Json;
using Parsing.RabbitMq.FinishParser;
using Parsing.RabbitMq.StartParsing;

namespace Avito.Vehicles.Service;

public class Worker(
    Serilog.ILogger logger,
    IStartParsingListener listener,
    IParserFinishMessagePublisher finishedPublisher
) : BackgroundService
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await listener.Prepare(cancellationToken);
        logger.Information("Worker service started.");
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        listener.Consumer.ReceivedAsync += async (sender, args) =>
        {
            logger.Information("Message received");
            ParserStartedRabbitMqMessage? parserStarted =
                JsonSerializer.Deserialize<ParserStartedRabbitMqMessage>(args.Body.ToArray());
            if (parserStarted != null)
            {
                ParserFinishedMessage finished = new ParserFinishedMessage(
                    parserStarted.ParserName,
                    parserStarted.ParserType,
                    10000
                );
                await finishedPublisher.Publish(finished);
                logger.Information("Sended message to publish.");
            }
            await listener.Acknowledge(args, stoppingToken);
        };
        await listener.StartConsuming(stoppingToken);
        logger.Information("Worker service started consuming.");
        stoppingToken.ThrowIfCancellationRequested();
    }
}
