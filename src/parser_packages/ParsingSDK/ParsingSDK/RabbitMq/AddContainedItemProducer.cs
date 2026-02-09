using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ParsingSDK.RabbitMq;

public sealed class AddContainedItemProducer(RabbitMqConnectionSource rabbitMq, Serilog.ILogger logger)
{
    private const string Exchange = "contained-items";
    private const string RoutingKey = "contained-items.create";
    private readonly RabbitMqProducer _producer = new(logger, rabbitMq);
    
    public async Task Publish(AddContainedItemsMessage message, CancellationToken ct = default)
    {
        RabbitMqPublishOptions options = new() { Persistent = true };
        await _producer.PublishDirectAsync(message, Exchange, RoutingKey, options, ct);
    }
}