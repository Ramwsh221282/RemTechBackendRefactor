using RabbitMQ.Client;
using RemTech.SharedKernel.Infrastructure.RabbitMq.Publishers;
using RemTech.SharedKernel.Infrastructure.RabbitMq.Publishers.Topic;

namespace RemTech.SharedKernel.Infrastructure.RabbitMq.Shared;

public sealed class RabbitMqPublishingMethod
{
    public async Task<PublishDeliveryInfo> Publish(
        RabbitMqConnectionSource connectionSource,
        string message, 
        string queue, 
        string exchange, 
        string routingKey, 
        CancellationToken ct = default)
    {
        IConnection connection = await connectionSource.GetConnection(ct);
        TopicRabbitMqPublisher publisher = new(connection);
        TopicPublishOptions options = new(message, queue, exchange, routingKey);
        PublishDeliveryInfo delivery = await publisher.Publish(options, ct);
        return delivery;
    }
}