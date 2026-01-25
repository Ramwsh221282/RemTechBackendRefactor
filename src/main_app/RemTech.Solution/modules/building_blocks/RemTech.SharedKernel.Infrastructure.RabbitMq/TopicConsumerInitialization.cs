using RabbitMQ.Client;

namespace RemTech.SharedKernel.Infrastructure.RabbitMq;

public static class TopicConsumerInitialization
{
    public static async Task<IChannel> InitializeChannel(
        RabbitMqConnectionSource connectionSource,
        string exchangeName,
        string queueName,
        string routingKey,
        CancellationToken ct
    )
    {
        IConnection connection = await connectionSource.GetConnection(ct);
        IChannel channel = await connection.CreateChannelAsync(cancellationToken: ct);
        await DeclareExchange(channel, exchangeName);
        await DeclareQueue(channel, queueName);
        await BindQueue(channel, queueName, exchangeName, routingKey);
        return channel;
    }

    private static Task DeclareExchange(IChannel channel, string exchangeName) =>
        channel.ExchangeDeclareAsync(exchange: exchangeName, type: "topic", durable: true, autoDelete: false);

    private static Task<QueueDeclareOk> DeclareQueue(IChannel channel, string queueName) =>
        channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);

    private static Task BindQueue(IChannel channel, string queueName, string exchangeName, string routingKey) =>
        channel.QueueBindAsync(queue: queueName, exchange: exchangeName, routingKey: routingKey);
}
