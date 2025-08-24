using System.Diagnostics;
using System.Text.Json;
using Avito.Vehicles.Service.VehiclesParsing;
using Parsing.Cache;
using Parsing.Grpc.Services.DuplicateIds;
using Parsing.RabbitMq.AddJournalRecord;
using Parsing.RabbitMq.Facade;
using Parsing.RabbitMq.PublishVehicle;
using Parsing.RabbitMq.PublishVehicle.Extras;
using Parsing.RabbitMq.StartParsing;
using Parsing.SDK.Browsers;
using Parsing.SDK.ScrapingActions;
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
    IDisabledScraperTracker disabledTracker,
    IAddJournalRecordPublisher addJournalRecord,
    GrpcDuplicateIdsClient client
) : BackgroundService
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await client.Ping();
        }
        catch
        {
            logger.Fatal("Unable to connect to Grpc backend.");
            throw;
        }
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
        await using IScrapingBrowser browser = await BrowserFactory.ProvideBrowser();
        await using IPage page = await browser.ProvideDefaultPage();
        Stopwatch parserStopwatch = new Stopwatch();
        parserStopwatch.Start();
        await addJournalRecord.PublishJournalRecord(
            parserStarted.ParserName,
            parserStarted.ParserType,
            "Работа парсера",
            "Парсинг начался."
        );
        try
        {
            foreach (var link in parserStarted.Links)
            {
                if (await HasPermantlyDisabled(parserStarted))
                {
                    await addJournalRecord.PublishJournalRecord(
                        parserStarted.ParserName,
                        parserStarted.ParserType,
                        "Работа парсера",
                        $"Парсер был немедленно отключен. Остановка работы."
                    );
                }
                await addJournalRecord.PublishJournalRecord(
                    parserStarted.ParserName,
                    parserStarted.ParserType,
                    "Обработка ссылки",
                    $"Обработка ссылки {link.LinkUrl}. Начата."
                );
                Stopwatch parserLinkStopwatch = new Stopwatch();
                parserLinkStopwatch.Start();
                await new PageNavigating(page, link.LinkUrl).Do();
                IParsedVehicleSource vehicleSource = new AvitoVehiclesParser(
                    page,
                    new NoTextWrite(),
                    logger,
                    link.LinkUrl,
                    client
                );
                await foreach (IParsedVehicle vehicle in vehicleSource.Iterate())
                {
                    await addJournalRecord.PublishJournalRecord(
                        parserStarted.ParserName,
                        parserStarted.ParserType,
                        "Обработка объявления",
                        $"Обработка объявления начата."
                    );
                    if (await HasPermantlyDisabled(parserStarted))
                    {
                        await addJournalRecord.PublishJournalRecord(
                            parserStarted.ParserName,
                            parserStarted.ParserType,
                            "Работа парсера",
                            $"Парсер был немедленно отключен. Остановка работы."
                        );
                        break;
                    }
                    if (!await new ValidatingParsedVehicle(vehicle).IsValid())
                    {
                        await addJournalRecord.PublishJournalRecord(
                            parserStarted.ParserName,
                            parserStarted.ParserType,
                            "Обработка объявления",
                            $"Объявление пропущено. Проблема с данными объявления."
                        );
                        continue;
                    }
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
                    await addJournalRecord.PublishJournalRecord(
                        parserStarted.ParserName,
                        parserStarted.ParserType,
                        "Обработка объявления",
                        $"Объявление {message.Vehicle.SourceUrl} добавлено в очередь добавления."
                    );
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
                await addJournalRecord.PublishJournalRecord(
                    parserStarted.ParserName,
                    parserStarted.ParserType,
                    "Обработка ссылки",
                    $"Обработка ссылки {link.LinkUrl}. Закончена."
                );
            }
        }
        catch (Exception ex)
        {
            logger.Fatal("{Ex}.", ex.Message);
            await addJournalRecord.PublishJournalRecord(
                parserStarted.ParserName,
                parserStarted.ParserType,
                "Работа парсера",
                $"Ошибка выполнения парсера. {ex.Message}. Работа закончена."
            );
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
        await addJournalRecord.PublishJournalRecord(
            parserStarted.ParserName,
            parserStarted.ParserType,
            "Работа парсера",
            $"Парсер закончил работу."
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
