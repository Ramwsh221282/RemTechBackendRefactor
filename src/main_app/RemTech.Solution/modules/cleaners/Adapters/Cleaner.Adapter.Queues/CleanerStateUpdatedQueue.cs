using System.Text.Json;
using Cleaners.Domain.Cleaners.UseCases.UpdateWorkStatistics;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Events;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.Consumers;
using Shared.Infrastructure.Module.DependencyInjection;

namespace Cleaner.Adapter.Queues;

public sealed class CleanerStateUpdatedQueue(
    RabbitMqConnectionProvider provider,
    Serilog.ILogger logger,
    IServiceProvider sp
) : BaseExchangedRabbitMqListener(provider)
{
    private const string QueueName = "CleanerStateUpdatedEvent";
    private const string ExchangeName = "Cleaners";

    public override void Configure()
    {
        Configurer.Queue.WithName(QueueName);
        Configurer.Exchange.WithName(ExchangeName);
        logger.Information("Configured {Queue} with {Exchange}.", QueueName, ExchangeName);
    }

    public override async Task HandleMessage(object sender, BasicDeliverEventArgs eventArgs)
    {
        var @event = GetEventInfo(eventArgs);
        if (@event == null)
        {
            logger.Information("{Queue}. Received invalid event message.", QueueName);
            await Acknowledge(eventArgs);
            return;
        }

        logger.Information(
            "{Queue} attempt to handle {Event}.",
            QueueName,
            nameof(CleanerStateUpdatedEvent)
        );

        await HandleCommand(@event);
        logger.Information("{Queue} handled event.", QueueName);
        await Acknowledge(eventArgs);
    }

    private static CleanerStateUpdatedEvent? GetEventInfo(BasicDeliverEventArgs eventArgs)
    {
        var body = eventArgs.Body;
        var @event = JsonSerializer.Deserialize<CleanerStateUpdatedEvent>(body.Span);
        return @event;
    }

    private async Task HandleCommand(CleanerStateUpdatedEvent @event)
    {
        var command = new UpdateWorkStatisticsCommand(
            @event.Id,
            @event.ElapsedSeconds,
            @event.ProcessedAmount
        );

        await using var scope = sp.CreateAsyncScope();

        var result = await scope
            .GetService<
                ICommandHandler<
                    UpdateWorkStatisticsCommand,
                    Status<Cleaners.Domain.Cleaners.Aggregate.Cleaner>
                >
            >()
            .Handle(command);

        if (result.IsFailure)
            logger.Error(
                "{Queue} handled message with error: {Error}.",
                QueueName,
                result.Error.ErrorText
            );
        else
            logger.Information("{Queue} handled message.", QueueName);
    }

    private sealed record CleanerStateUpdatedEvent(
        Guid Id,
        long ElapsedSeconds,
        int ProcessedAmount
    );
}
