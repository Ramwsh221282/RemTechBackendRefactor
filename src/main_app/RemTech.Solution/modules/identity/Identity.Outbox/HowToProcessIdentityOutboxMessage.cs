using RemTech.Outbox.Shared;
using RemTech.RabbitMq.Abstractions.Publishers;
using RemTech.RabbitMq.Abstractions.Publishers.Topic;

namespace Identity.Outbox;

public sealed class HowToProcessIdentityOutboxMessage : IHowToProcessIdentityOutboxMessage
{
    private readonly RabbitMqPublishers _publishers;
    
    public async Task ProcessMessage(ProcessedOutboxMessages messages, OutboxMessage message, CancellationToken ct = default)
    {
        string body = message.Body;
        string queue = message.Queue;
        string exchange = message.Exchange;
        string routingKey = message.RoutingKey;
        IRabbitMqPublisher<TopicPublishOptions> publisher = await _publishers.TopicPublisher(ct);
        TopicPublishOptions options = new(body, queue, exchange, routingKey);
        await publisher.Publish(options, ct);
        messages.Add(message);
    }
    
    public HowToProcessIdentityOutboxMessage(RabbitMqPublishers publishers)
    {
        _publishers = publishers;
    }
}