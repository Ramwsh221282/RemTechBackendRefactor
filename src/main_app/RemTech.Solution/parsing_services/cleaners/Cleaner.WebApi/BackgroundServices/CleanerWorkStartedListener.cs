using Cleaner.WebApi.Events;
using Cleaner.WebApi.ExternalSources;
using Cleaner.WebApi.Messages;
using Cleaner.WebApi.Messaging;
using Cleaner.WebApi.Models;
using Cleaner.WebApi.Storages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Infrastructure.Module.Consumers;
using Shared.Infrastructure.Module.DependencyInjection;

namespace Cleaner.WebApi.BackgroundServices;

public sealed class CleanerWorkStartedListener(
    RabbitMqConnectionProvider provider,
    Serilog.ILogger logger,
    IProcessingItemsSource externalItems,
    IWorkingCleanersStorage cleaners,
    IServiceProvider sp
) : BaseExchangedRabbitMqListener(provider)
{
    private const string QueueName = "CleanerWorkStarted";
    private const string ExchangeName = "Cleaners";
    private const int BatchSize = 100;

    public override void Configure()
    {
        Configurer.Queue.WithName(QueueName);
        Configurer.Exchange.WithName(ExchangeName).WithType(ExchangeType.Direct);
        logger.Information(
            "Queue: {Queue} Exchange: {Exchange} has been configured.",
            QueueName,
            ExchangeName
        );
    }

    public override async Task HandleMessage(object sender, BasicDeliverEventArgs eventArgs)
    {
        try
        {
            await Handle(eventArgs);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Queue}. Fatal at message processing. {Exception}", QueueName, ex);
        }
    }

    private async Task Handle(BasicDeliverEventArgs eventArgs)
    {
        logger.Information("{QueueName} received message.", QueueName);

        // прочитать сообщение
        var cleaner = GetWorkingCleaner(eventArgs);
        // получить элементы для очистки.
        var itemsToProcess = await externalItems.GetItemsForCleaner(cleaner, BatchSize);

        // если нет элементов, подтверждаем, что работа выполнена.
        if (!itemsToProcess.Any())
        {
            logger.Warning(
                "{QueueName} no items to clean found. Current cleaner threshold: {Threshold}.",
                QueueName,
                cleaner.ItemsDateDayThreshold
            );
            await PublishCleanerWorkFinished(cleaner, sp);
            return;
        }

        // если элементы есть, заносим в кеш чистильщика для дальнейшей обработки.
        await cleaners.Invalidate(cleaner);
        await Acknowledge(eventArgs);
    }

    private static WorkingCleaner GetWorkingCleaner(BasicDeliverEventArgs eventArgs)
    {
        var message = CleanerWorkStarted.FromMessageBody(eventArgs.Body);
        var cleaner = message.ToWorkingCleaner();
        return cleaner;
    }

    private static async Task PublishCleanerWorkFinished(
        WorkingCleaner cleaner,
        IServiceProvider sp
    )
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
}
