using System.Diagnostics;
using System.Text.Json;
using Drom.Parsing.Vehicles.Parsing.AttributeParsers;
using Drom.Parsing.Vehicles.Parsing.Models;
using Drom.Parsing.Vehicles.Parsing.Utilities;
using Parsing.Cache;
using Parsing.RabbitMq.Facade;
using Parsing.RabbitMq.StartParsing;
using Parsing.SDK.Browsers;
using Parsing.SDK.Browsers.BrowserLoadings;
using Parsing.SDK.Browsers.InstantiationModes;
using Parsing.SDK.Browsers.InstantiationOptions;
using Parsing.SDK.Browsers.PageSources;
using Parsing.SDK.ScrapingActions;
using PuppeteerSharp;
using RabbitMQ.Client.Events;

namespace Drom.Parsing.Vehicles;

public class Worker(
    Serilog.ILogger logger,
    IStartParsingListener listener,
    IParserRabbitMqActionsPublisher publisher,
    IDisabledScraperTracker disabledTracker
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
        Stopwatch parserStopwatch = new Stopwatch();
        parserStopwatch.Start();
        try
        {
            foreach (var link in parserStarted.Links)
            {
                if (await HasPermantlyDisabled(parserStarted))
                {
                    break;
                }

                Stopwatch parserLinkStopwatch = new Stopwatch();
                parserLinkStopwatch.Start();
                await using IScrapingBrowser browser = new SinglePagedScrapingBrowser(
                    await new DefaultBrowserInstantiation(
                        new HeadlessBrowserInstantiationOptions(),
                        new BasicBrowserLoading()
                    ).Instantiation(),
                    link.LinkUrl
                );
                await using IBrowserPagesSource pagesSource = await browser.AccessPages();
                foreach (IPage page in pagesSource.Iterate())
                {
                    if (await HasPermantlyDisabled(parserStarted))
                    {
                        break;
                    }

                    DromPagesCursor cursor = new DromPagesCursor(page);
                    bool shouldStop = false;
                    while (!shouldStop)
                    {
                        if (await HasPermantlyDisabled(parserStarted))
                        {
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
                            if (await HasPermantlyDisabled(parserStarted))
                            {
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
                            item.LogMessage().Log(logger);
                            if (item.Valid())
                            {
                                await publisher.SayVehicleFinished(
                                    item.AsPublishMessage(
                                        parserStarted.ParserName,
                                        parserStarted.ParserType,
                                        parserStarted.ParserDomain,
                                        link.LinkName,
                                        link.LinkUrl
                                    )
                                );
                            }
                        }
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
