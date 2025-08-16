using System.Diagnostics;
using System.Text.Json;
using Avito.Vehicles.Service.VehiclesParsing;
using Parsing.Cache;
using Parsing.RabbitMq.Facade;
using Parsing.RabbitMq.PublishVehicle;
using Parsing.RabbitMq.PublishVehicle.Extras;
using Parsing.RabbitMq.StartParsing;
using Parsing.SDK.Browsers;
using Parsing.Vehicles.Common.ParsedVehicles;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePhotos;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePrices;
using Parsing.Vehicles.Common.TextWriting;
using PuppeteerSharp;
using RabbitMQ.Client.Events;

namespace Avito.Vehicles.Service;

public class Worker(
    Serilog.ILogger logger,
    IStartParsingListener listener,
    IParserRabbitMqActionsPublisher publisher,
    IDisabledScraperTracker disabledTracker
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
        listener.Consumer.ReceivedAsync += HandleMessage;
        await listener.StartConsuming(stoppingToken);
        logger.Information("Worker service started consuming.");
        stoppingToken.ThrowIfCancellationRequested();
    }

    private async Task HandleMessage(object sender, BasicDeliverEventArgs eventArgs)
    {
        await listener.Acknowledge(eventArgs);
        ParserStartedRabbitMqMessage? parserStarted =
            JsonSerializer.Deserialize<ParserStartedRabbitMqMessage>(eventArgs.Body.ToArray());
        if (parserStarted == null)
            return;
        await using IScrapingBrowser browser = await BrowserFactory.ProvideDevelopmentBrowser();
        await using IPage page = await browser.ProvideDefaultPage();
        Stopwatch parserStopwatch = new Stopwatch();
        parserStopwatch.Start();
        try
        {
            foreach (var link in parserStarted.Links)
            {
                if (await HasPermantlyDisabled(parserStarted))
                    break;
                Stopwatch parserLinkStopwatch = new Stopwatch();
                parserLinkStopwatch.Start();
                if (await HasPermantlyDisabled(parserStarted))
                    break;

                IParsedVehicleSource vehicleSource = new AvitoVehiclesParser(
                    page,
                    new NoTextWrite(),
                    logger,
                    link.LinkUrl
                );
                await foreach (IParsedVehicle vehicle in vehicleSource.Iterate())
                {
                    if (await HasPermantlyDisabled(parserStarted))
                        break;
                    if (!await new ValidatingParsedVehicle(vehicle).IsValid())
                        continue;
                    ParsedVehiclePrice price = await vehicle.Price();
                    CharacteristicsDictionary ctxes = await vehicle.Characteristics();
                    UniqueParsedVehiclePhotos photos = await vehicle.Photos();
                    SentencesCollection sentences = await vehicle.Sentences();
                    string description = sentences.FormText();
                    if (string.IsNullOrWhiteSpace(description))
                    {
                        logger.Warning("No description provided.");
                        continue;
                    }
                    VehiclePublishMessage message = new VehiclePublishMessage(
                        new ParserBody(
                            parserStarted.ParserName,
                            parserStarted.ParserType,
                            parserStarted.ParserDomain
                        ),
                        new ParserLinkBody(
                            parserStarted.ParserName,
                            parserStarted.ParserType,
                            parserStarted.ParserDomain,
                            link.LinkName,
                            link.LinkUrl
                        ),
                        new VehicleBody(
                            await vehicle.Identity(),
                            await vehicle.Kind(),
                            await vehicle.Brand(),
                            await vehicle.Model(),
                            price,
                            price.IsNds(),
                            await vehicle.Geo(),
                            await vehicle.SourceUrl(),
                            description,
                            ctxes
                                .Read()
                                .Select(c => new VehicleBodyCharacteristic(c.Name(), c.Value())),
                            photos.Read().Select(p => new VehicleBodyPhoto(p))
                        )
                    );
                    await publisher.SayVehicleFinished(message);
                }
                parserLinkStopwatch.Stop();
                long linkSeconds = (long)parserLinkStopwatch.Elapsed.TotalSeconds;
                await publisher.SayParserLinkFinished(
                    parserStarted.ParserName,
                    parserStarted.ParserType,
                    link.LinkName,
                    linkSeconds
                );
                logger.Information("Sended finish link {Link}.", link.LinkName);
            }
        }
        catch (Exception ex)
        {
            logger.Fatal("{Ex}.", ex.Message);
        }
        parserStopwatch.Stop();
        long parserStopwatchSeconds = (long)parserStopwatch.Elapsed.TotalSeconds;
        await publisher.SayParserFinished(
            parserStarted.ParserName,
            parserStarted.ParserType,
            parserStopwatchSeconds
        );
        logger.Information(
            "Sended finish parser {Name} {Type}",
            parserStarted.ParserName,
            parserStarted.ParserType
        );
    }

    private async Task<bool> HasPermantlyDisabled(ParserStartedRabbitMqMessage parser)
    {
        bool disabled = await disabledTracker.HasBeenDisabled(parser.ParserName, parser.ParserType);
        if (disabled)
        {
            logger.Warning(
                "{Parser} {Type} has been permantly disabled. Stopping process.",
                parser.ParserName,
                parser.ParserType
            );
        }
        return disabled;
    }
}
