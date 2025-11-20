using System.Text;
using RabbitMQ.Client;

namespace RemTech.RabbitMq.Abstractions.Publishers.Topic;

public sealed class TopicRabbitMqPublisher : IRabbitMqPublisher<TopicPublishOptions>
{
    private readonly IConnection _connection;

    public TopicRabbitMqPublisher(IConnection connection)
    {
        _connection = connection;
    }

    public async Task<PublishDeliveryInfo> Publish(TopicPublishOptions options, CancellationToken ct = default)
    {
        await using IChannel channel = await _connection.CreateChannelAsync(cancellationToken: ct);
        await DeclareQueue(channel, options, ct);
        await DeclareExchange(channel, options, ct);
        await BindQueue(channel, options, ct);
        return await InvokePublishing(channel, options, ct);
    }

    private async Task DeclareQueue(IChannel channel, TopicPublishOptions options, CancellationToken ct = default)
    {
        await channel.QueueDeclareAsync(
            options.Queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            passive: false,
            noWait: false,
            cancellationToken: ct);
    }

    private async Task DeclareExchange(IChannel channel, TopicPublishOptions options, CancellationToken ct = default)
    {
        await channel.ExchangeDeclareAsync(
            options.Exchange,
            "topic",
            durable: true,
            autoDelete: false,
            arguments: null,
            passive: false,
            noWait: false,
            cancellationToken: ct);
    }

    private async Task BindQueue(IChannel channel, TopicPublishOptions options, CancellationToken ct = default)
    {
        await channel.QueueBindAsync(
            options.Queue,
            options.Exchange,
            options.RoutingKey,
            arguments: null,
            cancellationToken: ct);
    }

    private async Task<PublishDeliveryInfo> InvokePublishing(IChannel channel, TopicPublishOptions options, CancellationToken ct = default)
    {
        string correlationId = Guid.NewGuid().ToString();
        string messageId = Guid.NewGuid().ToString();
        BasicProperties properties = new()
        {
            Persistent = true,
            CorrelationId = correlationId,
            MessageId = messageId
        };
        ReadOnlyMemory<byte> body = Encoding.UTF8.GetBytes(options.Message);
        await channel.BasicPublishAsync(
            options.Exchange, 
            options.RoutingKey, 
            mandatory: false, 
            properties, body,
            cancellationToken: ct);
        return new PublishDeliveryInfo(correlationId, messageId);
    }
}