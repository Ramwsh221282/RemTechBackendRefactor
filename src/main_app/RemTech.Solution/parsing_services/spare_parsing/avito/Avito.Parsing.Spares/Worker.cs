using System.Diagnostics;
using System.Text.Json;
using Avito.Parsing.Spares.Parsing;
using Parsing.Avito.Common.BypassFirewall;
using Parsing.Avito.Common.PaginationBar;
using Parsing.Cache;
using Parsing.RabbitMq.Facade;
using Parsing.RabbitMq.PublishSpare;
using Parsing.RabbitMq.PublishVehicle;
using Parsing.RabbitMq.StartParsing;
using Parsing.SDK.Browsers;
using Parsing.SDK.Browsers.BrowserLoadings;
using Parsing.SDK.Browsers.InstantiationModes;
using Parsing.SDK.Browsers.InstantiationOptions;
using Parsing.SDK.Browsers.PageSources;
using Parsing.SDK.ScrapingActions;
using PuppeteerSharp;
using RabbitMQ.Client.Events;
using RemTech.Core.Shared.Decorating;

namespace Avito.Parsing.Spares;

public sealed class Worker(
    Serilog.ILogger logger,
    IStartParsingListener listener,
    IParserRabbitMqActionsPublisher publisher,
    IDisabledScraperTracker disabledTracker
) : BackgroundService
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await listener.Prepare(cancellationToken);
        logger.Information("Worker service starting...");
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        listener.Consumer.ReceivedAsync += HandleMessage;
        await listener.StartConsuming(stoppingToken);
        logger.Information("Worker service started consuming.");
        stoppingToken.ThrowIfCancellationRequested();
    }

    private async Task HandleMessage(object sender, BasicDeliverEventArgs ea)
    {
        await listener.Acknowledge(ea);
        ParserStartedRabbitMqMessage? parserStarted =
            JsonSerializer.Deserialize<ParserStartedRabbitMqMessage>(ea.Body.ToArray());
        if (parserStarted == null)
            return;
        Stopwatch parserStopwatch = new Stopwatch();
        parserStopwatch.Start();
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
            IBrowserPagesSource pages = await browser.AccessPages();
            foreach (IPage page in pages.Iterate())
            {
                if (await HasPermantlyDisabled(parserStarted))
                {
                    break;
                }

                IAvitoBypassFirewall bypass = new AvitoBypassFirewall(page)
                    .WrapBy(p => new AvitoBypassFirewallExceptionSupressing(p))
                    .WrapBy(p => new AvitoBypassFirewallLazy(page, p))
                    .WrapBy(p => new AvitoBypassRepetetive(page, p))
                    .WrapBy(p => new AvitoBypassWebsiteIsNotAvailable(page, p))
                    .WrapBy(p => new AvitoBypassFirewallLogging(logger, p));
                IAvitoPaginationBarSource pagination = new AvitoPaginationBarSource(page)
                    .WrapBy(p => new BottomScrollingAvitoPaginationBarSource(page, p))
                    .WrapBy(p => new LoggingAvitoPaginationBarSource(logger, p));
                if (!await bypass.Read())
                    break;
                LoggingAvitoPaginationBarElement bar = new LoggingAvitoPaginationBarElement(
                    logger,
                    await pagination.Read()
                );
                SpareBodyValidator validator = new SpareBodyValidator();
                foreach (string pageUrl in bar.Iterate(link.LinkUrl))
                {
                    if (await HasPermantlyDisabled(parserStarted))
                    {
                        break;
                    }

                    await new PageNavigating(page, pageUrl)
                        .WrapBy(n => new LoggingPageNavigating(logger, pageUrl, n))
                        .Do();
                    IEnumerable<AvitoSpare> spares = await new BlockBypassingAvitoSparesCollection(
                        bypass,
                        new AvitoSparesCollection(page)
                    ).Read();
                    foreach (AvitoSpare spare in spares)
                    {
                        if (await HasPermantlyDisabled(parserStarted))
                        {
                            break;
                        }

                        await spare.Navigate(page);
                        if (!await bypass.Read())
                            continue;
                        await new PageBottomScrollingAction(page).Do();
                        await new PageUpperScrollingAction(page).Do();
                        await new VariantDescriptionDetailsSource()
                            .With(new AvitoCharacteristicsDetailsSource(page))
                            .With(new AvitoDescriptionDetailsSource(page))
                            .Add(spare);
                        SpareBody body = spare.AsSpareBody();
                        if (!validator.IsValid(body))
                            continue;
                        await publisher.SaySparePublished(
                            new SpareSinkMessage(
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
                                body
                            )
                        );
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
            logger.Information("Sended finish link {Link}", link.LinkName);
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
