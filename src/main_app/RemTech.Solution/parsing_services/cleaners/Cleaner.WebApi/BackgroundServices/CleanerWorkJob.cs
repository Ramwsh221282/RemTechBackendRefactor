using System.Diagnostics;
using Cleaner.WebApi.CleaningLogic;
using Cleaner.WebApi.Events;
using Cleaner.WebApi.ExternalSources;
using Cleaner.WebApi.Messaging;
using Cleaner.WebApi.Models;
using Cleaner.WebApi.Storages;
using Parsing.SDK.Browsers;
using PuppeteerSharp;
using Quartz;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.DependencyInjection;

namespace Cleaner.WebApi.BackgroundServices;

public sealed class CleanerWorkJob(
    Serilog.ILogger logger,
    IWorkingCleanersStorage cleaners,
    WorkTimeStorage workTime,
    IProcessingItemsSource externalItems,
    IServiceProvider sp
) : IJob
{
    private const string Context = nameof(CleanerWorkJob);

    public async Task Execute(IJobExecutionContext context)
    {
        logger.Information("{Context}. Attempt to process items.", Context);
        int processedItems = 0;
        try
        {
            processedItems = await Handle();
        }
        catch (Exception ex)
        {
            logger.Fatal("{Context}. Error at cleaning items: {Ex}.", Context, ex.Message);
        }

        logger.Information("{Context}. {Amount} Items processed.", Context, processedItems);
    }

    private async Task<int> Handle()
    {
        var cleaner = await cleaners.Get();
        if (cleaner == null)
        {
            logger.Warning("{Context}. Lacking of processing cleaner.", Context);
            return 0;
        }

        var items = await externalItems.GetItemsForCleaner(cleaner, 100);
        if (!items.Any())
        {
            await PublishWorkFinishedEvent(cleaner);
            await cleaners.Remove();
            return 0;
        }

        Stopwatch sw = Stopwatch.StartNew();
        await using var browser = await BrowserFactory.ProvideBrowser();
        var processedItemIds = await GetProcessedItems(browser, items);
        sw.Stop();

        await workTime.Invalidate(sw);
        cleaner.CleanedAmount += processedItemIds.Count();
        cleaner.TotalElapsedSeconds += await workTime.GetTotalElapsed();
        await cleaners.Invalidate(cleaner);

        await PublishCleanedItems(processedItemIds);
        await PublishCleanerState(cleaner);
        return processedItemIds.Count();
    }

    private async Task PublishWorkFinishedEvent(WorkingCleaner cleaner)
    {
        await using var scope = sp.CreateAsyncScope();
        var publisher = scope.GetService<ICleanerWorkFinishedEventPublisher>();

        var @event = new CleanerWorkFinishedEvent(
            cleaner.Id,
            cleaner.TotalElapsedSeconds,
            cleaner.CleanedAmount
        );

        await publisher.Publish(@event);
    }

    private async Task PublishCleanerState(WorkingCleaner cleaner)
    {
        await using var scope = sp.CreateAsyncScope();
        var publisher = scope.GetService<ICleanerStateUpdatePublisher>();

        var @event = new CleanerStateUpdatedEvent(
            cleaner.Id,
            cleaner.TotalElapsedSeconds,
            cleaner.CleanedAmount
        );

        await publisher.Publish(@event);
    }

    private async Task PublishCleanedItems(IEnumerable<string> cleanedItems)
    {
        await using var scope = sp.CreateAsyncScope();
        var publisher = scope.GetService<ICleanedItemsEventPublisher>();

        var @event = new CleanerItemsCleanedEvent(cleanedItems);

        await publisher.Publish(@event);
    }

    private async Task<IEnumerable<string>> GetProcessedItems(
        IScrapingBrowser browser,
        IEnumerable<CleanerProcessingItem> items
    )
    {
        List<string> processed = [];
        foreach (var item in items)
        {
            await using var page = await browser.ProvideDefaultPage();
            var processing = await ProcessItem(item, page);
            if (processing.IsFailure)
            {
                item.Status = CleanerProcessingItem.Errored;
                continue;
            }

            processed.Add(processing.Value);
        }

        return processed;
    }

    private async Task<Status<string>> ProcessItem(
        CleanerProcessingItem item,
        IPage page,
        int retryCount = 5
    )
    {
        var currentAttempt = 0;
        while (currentAttempt < retryCount)
        {
            try
            {
                var suspicious = new SuspiciousItem(item.Id, item.SourceUrl, item.Domain);
                var challenge = suspicious.SpecifyChallenge(page, logger);
                var isOutdated = await challenge.ItemIsOutdated();
                return isOutdated;
            }
            catch
            {
                currentAttempt++;
            }
        }

        return Error.Conflict("Unable to process suspicious item");
    }
}
