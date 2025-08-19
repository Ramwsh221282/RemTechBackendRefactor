using System.Diagnostics;
using System.Text.Json;
using Drom.Parsing.Vehicles.Parsing.AttributeParsers;
using Drom.Parsing.Vehicles.Parsing.Models;
using Drom.Parsing.Vehicles.Parsing.Utilities;
using Parsing.Cache;
using Parsing.RabbitMq.AddJournalRecord;
using Parsing.RabbitMq.Facade;
using Parsing.RabbitMq.PublishVehicle;
using Parsing.RabbitMq.StartParsing;
using Parsing.SDK.Browsers;
using Parsing.SDK.ScrapingActions;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;
using RabbitMQ.Client.Events;

namespace Drom.Parsing.Vehicles;

public class Worker(
    Serilog.ILogger logger,
    IStartParsingListener listener,
    IParserRabbitMqActionsPublisher publisher,
    IDisabledScraperTracker disabledTracker,
    IAddJournalRecordPublisher addJournalRecord
) : BackgroundService
{
    public override async Task StartAsync(CancellationToken stoppingToken)
    {
        await listener.Prepare(stoppingToken);
        logger.Information("Worker service started.");
        await base.StartAsync(stoppingToken);
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
        await addJournalRecord.PublishJournalRecord(
            parserStarted.ParserName,
            parserStarted.ParserType,
            "Работа парсера",
            "Парсинг начался."
        );
        logger.Information("Started.");
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
                    break;
                }
                await addJournalRecord.PublishJournalRecord(
                    parserStarted.ParserName,
                    parserStarted.ParserType,
                    "Обработка ссылки",
                    $"Обработка ссылки {link.LinkUrl}. Начата."
                );
                await new PageNavigating(page, link.LinkUrl).Do();
                Stopwatch parserLinkStopwatch = new Stopwatch();
                parserLinkStopwatch.Start();
                DromPagesCursor cursor = new DromPagesCursor(page);
                bool shouldStop = false;
                while (!shouldStop)
                {
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
                    await cursor.Navigate();
                    await new PageBottomScrollingAction(page).Do();
                    await new PageUpperScrollingAction(page).Do();
                    if (!await new DromImagesHoveringAction(page).Do())
                    {
                        shouldStop = true;
                        continue;
                    }
                    DromCatalogueCarsCollection collection = new(page);
                    IEnumerable<DromCatalogueCar> cars = await collection.Iterate();
                    foreach (DromCatalogueCar item in cars)
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
                        await item.Navigation().Navigate(page);
                        await new DromVehicleModelSource(page).Print(item);
                        await new DromVehicleBrandSource(page).Print(item);
                        await new DromVehicleKindSource(page).Print(item);
                        await new CarDescriptionSource(page).Print(item);
                        (await new DromVehicleCharacteristicSource(page).Read()).Print(item);
                        (await new CarPriceSource(page).Read()).Print(item);
                        (await new CarLocationSource(page).Read()).Print(item);
                        if (await IsNotRelevant(page))
                        {
                            logger.Warning("Item was not relevant. Skipping item.");
                            await addJournalRecord.PublishJournalRecord(
                                parserStarted.ParserName,
                                parserStarted.ParserType,
                                "Обработка объявления",
                                $"Объявление пропущено. Неактуально."
                            );
                            continue;
                        }

                        item.LogMessage().Log(logger);
                        if (!item.Valid())
                        {
                            await addJournalRecord.PublishJournalRecord(
                                parserStarted.ParserName,
                                parserStarted.ParserType,
                                "Обработка объявления",
                                $"Объявление пропущено. Проблема с данными объявления."
                            );
                            continue;
                        }

                        VehiclePublishMessage message = item.AsPublishMessage(
                            parserStarted.ParserName,
                            parserStarted.ParserType,
                            parserStarted.ParserDomain,
                            link.LinkName,
                            link.LinkUrl
                        );
                        await publisher.SayVehicleFinished(message);
                        await addJournalRecord.PublishJournalRecord(
                            parserStarted.ParserName,
                            parserStarted.ParserType,
                            "Обработка объявления",
                            $"Объявление {message.Vehicle.SourceUrl} добавлено в очередь добавления."
                        );
                    }
                }
                parserLinkStopwatch.Stop();
                long linkSeconds = (long)parserLinkStopwatch.Elapsed.TotalSeconds;
                await publisher.SayParserLinkFinished(
                    parserStarted.ParserName,
                    parserStarted.ParserType,
                    link.LinkName,
                    linkSeconds
                );
                logger.Information("Sended finsih link {Link}", link.LinkName);
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

    private static async Task<bool> IsNotRelevant(IPage page)
    {
        string container = string.Intern(".ftldj60.css-1yado2t");
        IElementHandle? element = await page.QuerySelectorAsync(container);
        if (element == null)
            return false;
        IElementHandle? innerContainer = await element.QuerySelectorAsync(".ftldj61");
        if (innerContainer == null)
            return false;
        IElementHandle? irrelevantContainer = await innerContainer.QuerySelectorAsync(
            ".ebrtcvm0.css-qjhitd.e1u9wqx22"
        );
        if (irrelevantContainer == null)
            return false;
        IElementHandle? irrelevantElement = await irrelevantContainer.QuerySelectorAsync(
            ".css-1jba3gn.edsrp6u3"
        );
        if (irrelevantElement == null)
            return false;
        IElementHandle? irrelevantTextElement = await irrelevantElement.QuerySelectorAsync(
            ".css-1gp719b.edsrp6u2"
        );
        if (irrelevantTextElement == null)
            return false;
        string text = await new TextFromWebElement(irrelevantTextElement).Read();
        return text.Contains("Техника снята с продажи");
    }
}
