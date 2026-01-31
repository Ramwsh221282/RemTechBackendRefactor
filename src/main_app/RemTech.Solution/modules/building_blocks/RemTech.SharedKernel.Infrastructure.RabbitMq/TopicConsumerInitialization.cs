using RabbitMQ.Client;

namespace RemTech.SharedKernel.Infrastructure.RabbitMq;

/// <summary>
/// Инициализация потребителей топиков RabbitMQ.
/// </summary>
public static class TopicConsumerInitialization
{
	/// <summary>
	/// Инициализирует канал RabbitMQ для потребителя топика.
	/// </summary>
	/// <param name="connectionSource">Источник подключения к RabbitMQ.</param>
	/// <param name="exchangeName">Имя обменника RabbitMQ.</param>
	/// <param name="queueName">Имя очереди RabbitMQ.</param>
	/// <param name="routingKey">Маршрутизирующий ключ для связывания очереди с обменником.</param>
	/// <param name="ct">Токен отмены для прерывания операции.</param>
	/// <returns>Инициализированный канал RabbitMQ.</returns>
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

	private static Task DeclareExchange(IChannel channel, string exchangeName)
	{
		return channel.ExchangeDeclareAsync(exchange: exchangeName, type: "topic", durable: true, autoDelete: false);
	}

	private static Task<QueueDeclareOk> DeclareQueue(IChannel channel, string queueName)
	{
		return channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);
	}

	private static Task BindQueue(IChannel channel, string queueName, string exchangeName, string routingKey)
	{
		return channel.QueueBindAsync(queue: queueName, exchange: exchangeName, routingKey: routingKey);
	}
}
