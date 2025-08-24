using System.Diagnostics;
using System.Text.Json;
using Avito.Parsing.Spares.Parsing;
using Parsing.Avito.Common.BypassFirewall;
using Parsing.Avito.Common.PaginationBar;
using Parsing.Cache;
using Parsing.Grpc.Services.DuplicateIds;
using Parsing.RabbitMq.AddJournalRecord;
using Parsing.RabbitMq.Facade;
using Parsing.RabbitMq.PublishSpare;
using Parsing.RabbitMq.PublishVehicle;
using Parsing.RabbitMq.StartParsing;
using Parsing.SDK.Browsers;
using Parsing.SDK.ScrapingActions;
using PuppeteerSharp;
using RabbitMQ.Client.Events;
using RemTech.Core.Shared.Decorating;

namespace Avito.Parsing.Spares;

public sealed class Worker(
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

        await using IScrapingBrowser browser = await BrowserFactory.Create();
        Stopwatch parserStopwatch = new Stopwatch();
        parserStopwatch.Start();
        await addJournalRecord.PublishJournalRecord(
            parserStarted.ParserName,
            parserStarted.ParserType,
            "Работа парсера",
            "Парсинг начался."
        );
        foreach (var link in parserStarted.Links)
        {
            Stopwatch parserLinkStopwatch = new Stopwatch();
            parserLinkStopwatch.Start();
            await addJournalRecord.PublishJournalRecord(
                parserStarted.ParserName,
                parserStarted.ParserType,
                "Обработка ссылки",
                $"Обработка ссылки {link.LinkUrl}. Начата."
            );
            try
            {
                await using IPage page = await browser.ProvideDefaultPage();
                int itemsProcessed = 0;
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
                IAvitoBypassFirewall bypass = new AvitoBypassFirewall(page)
                    .WrapBy(p => new AvitoBypassFirewallExceptionSupressing(p))
                    .WrapBy(p => new AvitoBypassFirewallLazy(page, p))
                    .WrapBy(p => new AvitoBypassRepetetive(page, p))
                    .WrapBy(p => new AvitoBypassWebsiteIsNotAvailable(page, p))
                    .WrapBy(p => new AvitoBypassFirewallLogging(logger, p));
                IAvitoPaginationBarSource pagination = new AvitoPaginationBarSource(page)
                    .WrapBy(p => new BottomScrollingAvitoPaginationBarSource(page, p))
                    .WrapBy(p => new LoggingAvitoPaginationBarSource(logger, p));
                await new PageNavigating(page, link.LinkUrl).Do();
                if (!await bypass.Read())
                {
                    await addJournalRecord.PublishJournalRecord(
                        parserStarted.ParserName,
                        parserStarted.ParserType,
                        "Обработка ссылки",
                        $"Обработка ссылки {link.LinkUrl}. Блокировка. Пропуск."
                    );
                    continue;
                }
                LoggingAvitoPaginationBarElement bar = new LoggingAvitoPaginationBarElement(
                    logger,
                    await pagination.Read()
                );
                SpareBodyValidator validator = new SpareBodyValidator();
                foreach (string pageUrl in bar.Iterate(link.LinkUrl))
                {
                    try
                    {
                        if (await HasPermantlyDisabled(parserStarted))
                        {
                            await addJournalRecord.PublishJournalRecord(
                                parserStarted.ParserName,
                                parserStarted.ParserType,
                                "Обработка ссылки",
                                $"Обработка ссылки {link.LinkUrl}. Начата."
                            );
                            logger.Information("Diabled");
                            break;
                        }

                        await new PageNavigating(page, pageUrl)
                            .WrapBy(n => new LoggingPageNavigating(logger, pageUrl, n))
                            .Do();
                        if (!await bypass.Read())
                        {
                            await addJournalRecord.PublishJournalRecord(
                                parserStarted.ParserName,
                                parserStarted.ParserType,
                                "Обработка ссылки",
                                $"Обработка ссылки {link.LinkUrl}. Блокировка. Пропуск."
                            );
                            continue;
                        }

                        IEnumerable<AvitoSpare> spares =
                            await new BlockBypassingAvitoSparesCollection(
                                bypass,
                                new DuplicateFilteringAvitoSparesCollection(
                                    client,
                                    new AvitoSparesCollection(page)
                                )
                            ).Read();
                        foreach (AvitoSpare spare in spares)
                        {
                            if (await HasPermantlyDisabled(parserStarted))
                            {
                                await addJournalRecord.PublishJournalRecord(
                                    parserStarted.ParserName,
                                    parserStarted.ParserType,
                                    "Обработка ссылки",
                                    $"Обработка ссылки {link.LinkUrl}. Начата."
                                );
                                break;
                            }

                            await spare.Navigate(page);
                            if (!await bypass.Read())
                            {
                                await addJournalRecord.PublishJournalRecord(
                                    parserStarted.ParserName,
                                    parserStarted.ParserType,
                                    "Обработка ссылки",
                                    $"Обработка ссылки {link.LinkUrl}. Блокировка. Пропуск."
                                );
                                continue;
                            }

                            await new PageBottomScrollingAction(page).Do();
                            await new PageUpperScrollingAction(page).Do();
                            await new VariantDescriptionDetailsSource()
                                .With(new AvitoCharacteristicsDetailsSource(page))
                                .With(new AvitoDescriptionDetailsSource(page))
                                .Add(spare);
                            SpareBody body = spare.AsSpareBody();
                            if (!validator.IsValid(body))
                            {
                                await addJournalRecord.PublishJournalRecord(
                                    parserStarted.ParserName,
                                    parserStarted.ParserType,
                                    "Обработка объявления",
                                    $"Объявление пропущено. Проблема с данными объявления."
                                );
                                continue;
                            }

                            var message = new SpareSinkMessage(
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
                            );
                            await publisher.SaySparePublished(message);
                            await addJournalRecord.PublishJournalRecord(
                                parserStarted.ParserName,
                                parserStarted.ParserType,
                                "Обработка объявления",
                                $"Объявление {message.Spare.SourceUrl} добавлено в очередь добавления."
                            );
                            itemsProcessed += 1;
                            logger.Information("Processed items amount: {Count}", itemsProcessed);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Fatal("{Exception} at catalogue page processing.", ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Fatal("{Exception} at scraping.", ex.Message);
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
            await addJournalRecord.PublishJournalRecord(
                parserStarted.ParserName,
                parserStarted.ParserType,
                "Обработка ссылки",
                $"Обработка ссылки {link.LinkUrl}. Закончена."
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
