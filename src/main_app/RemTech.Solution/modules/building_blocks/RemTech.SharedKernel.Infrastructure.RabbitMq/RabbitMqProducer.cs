using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace RemTech.SharedKernel.Infrastructure.RabbitMq;

public sealed class RabbitMqProducer(Serilog.ILogger logger, RabbitMqConnectionSource connectionSource)
{
	private IChannel? Channel { get; set; }
	private SemaphoreSlim Semaphore { get; } = new(1);
	private Serilog.ILogger Logger { get; } = logger.ForContext<RabbitMqProducer>();

	public async Task PublishDirectAsync<T>(
		T message,
		string exchange,
		string routingKey,
		RabbitMqPublishOptions options,
		CancellationToken ct = default
	)
	{
		IConnection connection = await connectionSource.GetConnection(ct);
		IChannel channel = await GetChannel(connection, ct);
		ReadOnlyMemory<byte> body = SerializeMessage(message);
		BasicProperties properties = new() { Persistent = options.Persistent };

		Logger.Debug("Publishing message. Blocking until semaphore is available.");
		await Semaphore.WaitAsync(ct);
		try
		{
			await channel.BasicPublishAsync(
				exchange: exchange,
				routingKey: routingKey,
				mandatory: false,
				basicProperties: properties,
				body: body,
				cancellationToken: ct
			);
			Logger.Information("Message published.");
		}
		finally
		{
			Semaphore.Release();
			Logger.Debug("Semaphore released.");
		}
	}

	private static ReadOnlyMemory<byte> SerializeMessage<T>(T message)
	{
		string jsonMessage = JsonSerializer.Serialize(message);
		return Encoding.UTF8.GetBytes(jsonMessage);
	}

	private ValueTask<IChannel> GetChannel(IConnection connection, CancellationToken ct)
	{
		Logger.Debug("Getting channel.");
		if (Channel?.IsOpen == true)
		{
			Logger.Information("Channel is initialized and open. Returning existing instance.");
			return new ValueTask<IChannel>(Channel);
		}

		async ValueTask<IChannel> CreateAsync()
		{
			Logger.Information("Creating channel. Blocking until semaphore is available.");
			await Semaphore.WaitAsync(ct);
			try
			{
				Channel ??= await connection.CreateChannelAsync(cancellationToken: ct);
				Logger.Information("Channel created and stored.");
				return Channel;
			}
			finally
			{
				Semaphore.Release();
				Logger.Debug("Semaphore released.");
			}
		}

		return CreateAsync();
	}
}
