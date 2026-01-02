using RemTech.SharedKernel.Infrastructure.RabbitMq;
using Vehicles.Domain.Features.AddVehicle;

namespace Vehicles.Infrastructure.RabbitMq.Producers;

public sealed class OnVehiclesAddedProducer(RabbitMqProducer producer, Serilog.ILogger logger) : IOnVehiclesAddedEventPublisher
{
    private const string Exchange = "parsers";
    private const string RoutingKey = "parsers.increase_processed";
    private RabbitMqProducer Producer { get; } = producer;
    private Serilog.ILogger Logger { get; } = logger.ForContext<OnVehiclesAddedProducer>();
    
    public async Task Publish(Guid creatorId, int addedAmount, CancellationToken ct = default)
    {
        OnVehiclesAddedMessage message = new()
        {
            Id = creatorId,
            Amount = addedAmount
        };
        RabbitMqPublishOptions options = new() { Persistent = true };
        await Producer.PublishDirectAsync(message, Exchange, RoutingKey, options, ct: ct);
        Logger.Information("Published {Id}. Amout: {Amount}", creatorId, addedAmount);
    }

    private sealed class OnVehiclesAddedMessage
    {
        public Guid Id { get; set; }
        public int Amount { get; set; }
    }
}