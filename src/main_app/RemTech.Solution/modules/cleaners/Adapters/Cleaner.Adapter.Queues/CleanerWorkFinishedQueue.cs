using System.Text.Json;
using Cleaners.Domain.Cleaners.UseCases.StartWait;
using Cleaners.Domain.Cleaners.UseCases.UpdateWorkStatistics;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Events;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.Consumers;
using Shared.Infrastructure.Module.DependencyInjection;

namespace Cleaner.Adapter.Queues;

public sealed class CleanerWorkFinishedQueue(
    RabbitMqConnectionProvider provider,
    IServiceProvider sp,
    Serilog.ILogger logger
) : BaseExchangedRabbitMqListener(provider)
{
    private const string QueueName = "CleanerWorkFinishedEvent";
    private const string Exchange = "Cleaners";

    public override void Configure()
    {
        Configurer.Queue.WithName(QueueName);
        Configurer.Exchange.WithName(Exchange);
        logger.Information("Queue {Queue} with {Exchange} configured.", QueueName, Exchange);
    }

    public override async Task HandleMessage(object sender, BasicDeliverEventArgs eventArgs)
    {
        var @event = GetEventData(eventArgs);
        if (@event == null)
        {
            logger.Information("Queue {Queue}. Received invalid event message.", QueueName);
            await Acknowledge(eventArgs);
            return;
        }

        logger.Information("{Queue} received message for finishing cleaner work.", QueueName);
        logger.Information("{Queue} attempt to update statistics from finished event.", QueueName);
        await HandleStatisticsUpdate(@event);
        logger.Information("{Queue} attempt to move cleaner to waiting state.", QueueName);
        await HandleWaitingStateChange(@event);
        logger.Information("{Queue} finished event processing.", QueueName);
        await Acknowledge(eventArgs);
    }

    private CleanerWorkFinishedEventData? GetEventData(BasicDeliverEventArgs eventArgs)
    {
        var body = eventArgs.Body;
        var @event = JsonSerializer.Deserialize<CleanerWorkFinishedEventData>(body.Span);
        return @event;
    }

    private async Task HandleStatisticsUpdate(CleanerWorkFinishedEventData @event)
    {
        await using var scope = sp.CreateAsyncScope();
        var result = await scope
            .GetService<
                ICommandHandler<
                    UpdateWorkStatisticsCommand,
                    Status<Cleaners.Domain.Cleaners.Aggregate.Cleaner>
                >
            >()
            .Handle(
                new UpdateWorkStatisticsCommand(
                    @event.Id,
                    @event.TotalElapsedSeconds,
                    @event.ProcessedItems
                )
            );

        if (result.IsFailure)
            logger.Error(
                "{Queue} attempt to update statistics from finished event failed. Error: {ErrorMessage}",
                QueueName,
                result.Error.ErrorText
            );
        else
            logger.Information(
                "{Queue} statistics from finished event has been updated.",
                QueueName
            );
    }

    private async Task HandleWaitingStateChange(CleanerWorkFinishedEventData @event)
    {
        await using var scope = sp.CreateAsyncScope();
        var result = await scope
            .GetService<
                ICommandHandler<
                    StartWaitCommand,
                    Status<Cleaners.Domain.Cleaners.Aggregate.Cleaner>
                >
            >()
            .Handle(new StartWaitCommand(@event.Id));
        if (result.IsFailure)
            logger.Error(
                "{Queue} attempt to move cleaner to waiting state failed. Error: {ErrorMessage}.",
                QueueName,
                result.Error.ErrorText
            );
        else
            logger.Information("{Queue} cleaner has changed to waiting state.", QueueName);
    }

    private sealed record CleanerWorkFinishedEventData(
        Guid Id,
        long TotalElapsedSeconds,
        long ProcessedItems
    );
}
