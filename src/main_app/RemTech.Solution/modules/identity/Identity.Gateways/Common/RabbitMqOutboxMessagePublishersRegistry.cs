using Identity.Infrastructure.Outbox;

namespace Identity.Gateways.Common;

public sealed class RabbitMqOutboxMessagePublishersRegistry
{
    private readonly Dictionary<string, IRabbitMqOutboxMessagePublisher> _publishers = [];

    public void AddPublishers(IEnumerable<IRabbitMqOutboxMessagePublisher> publishers)
    {
        foreach (var publisher in publishers)
        {
            string supportedMessagesType = publisher.SupportedMessageType;
            if (!_publishers.TryAdd(supportedMessagesType, publisher))
                throw new ApplicationException(
                    $"{nameof(RabbitMqOutboxMessagePublishersRegistry)} already contains: {supportedMessagesType} publisher.");
        }
    }

    public bool HasPublisherForMessage(IdentityOutboxMessage message)
    {
        string messageType = message.Type;
        return _publishers.ContainsKey(messageType);
    }

    public async Task<PublishedOutboxMessage> PublishMessage(IdentityOutboxMessage message, CancellationToken ct)
    {
        await _publishers[message.Type].Publish(message.Payload, ct);
        return new PublishedOutboxMessage(message);
    }
}