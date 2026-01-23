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

	private static async Task DeclareExchange(IChannel channel, string exchangeName)
	{
		await channel.ExchangeDeclareAsync(exchange: exchangeName, type: "topic", durable: true, autoDelete: false);
	}

	private static async Task DeclareQueue(IChannel channel, string queueName)
	{
		await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);
	}

	private static async Task BindQueue(IChannel channel, string queueName, string exchangeName, string routingKey)
	{
		await channel.QueueBindAsync(queue: queueName, exchange: exchangeName, routingKey: routingKey);
	}
}
