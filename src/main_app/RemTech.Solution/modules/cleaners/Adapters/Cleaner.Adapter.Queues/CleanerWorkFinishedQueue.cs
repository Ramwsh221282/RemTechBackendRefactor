using System.Text.Json;
using Cleaners.Domain.Cleaners.UseCases.StartWait;
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
        var body = eventArgs.Body;
        var @event = JsonSerializer.Deserialize<CleanerWorkFinishedEventData>(body.Span);
        if (@event == null)
        {
            logger.Information("Queue {Queue}. Received invalid event message.");
            await Acknowledge(eventArgs);
            return;
        }

        await using var scope = sp.CreateAsyncScope();
        var command = new StartWaitCommand(@event.Id);
        var result = await scope
            .GetService<
                ICommandHandler<
                    StartWaitCommand,
                    Status<Cleaners.Domain.Cleaners.Aggregate.Cleaner>
                >
            >()
            .Handle(command);

        if (result.IsFailure)
        {
            logger.Information(
                "{Queue} handled message with error: {Error}.",
                QueueName,
                result.Error
            );
        }
        else
        {
            logger.Information("{Queue} handled message.", QueueName);
        }

        await Acknowledge(eventArgs);
    }

    private sealed record CleanerWorkFinishedEventData(
        Guid Id,
        long TotalElapsedSeconds,
        long ProcessedItems
    );
}
