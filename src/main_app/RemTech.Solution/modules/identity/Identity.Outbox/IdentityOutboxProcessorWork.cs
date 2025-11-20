using RemTech.Outbox.Shared;
using RemTech.RabbitMq.Abstractions.Publishers;
using RemTech.RabbitMq.Abstractions.Publishers.Topic;

namespace Identity.Outbox;

public sealed class IdentityOutboxProcessorWork : IIdentityOutboxProcessorWork
{
    private readonly OutboxService _service;
    private readonly RabbitMqPublishers _publishers;
    private readonly CancellationToken _ct;

    public async Task<ProcessedOutboxMessages> ProcessMessages()
    {
        IEnumerable<OutboxMessage> messages = await _service.GetPendingMessages(50, _ct);
        ProcessedOutboxMessages result = await PublishMessages(messages);
        await result.RemoveProcessedMessages(_service, _ct);
        return result;
    }

    private async Task<ProcessedOutboxMessages> PublishMessages(IEnumerable<OutboxMessage> messages)
    {
        ProcessedOutboxMessages result = new();
        
        foreach (OutboxMessage message in messages)
        {
            IRabbitMqPublisher<TopicPublishOptions> publisher = await _publishers.TopicPublisher();
            
            string body = message.Body;
            string queue = message.Queue;
            string exchange = message.Exchange;
            string routingKey = message.RoutingKey;
            
            TopicPublishOptions options = new(body, queue, exchange, routingKey);
            await publisher.Publish(options, _ct);
            result.Add(message);
        }
        return result;
    }
    
    public IdentityOutboxProcessorWork(OutboxService service, RabbitMqPublishers publishers, CancellationToken ct)
    {
        _service = service;
        _publishers = publishers;
        _ct = ct;
    }
}