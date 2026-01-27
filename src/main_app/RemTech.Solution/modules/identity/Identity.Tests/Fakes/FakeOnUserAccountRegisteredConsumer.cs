using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace Identity.Tests.Fakes;

/// <summary>
/// Фейковый потребитель сообщений о регистрации аккаунта пользователя.
/// </summary>
/// <param name="rabbitMq">Источник подключения к RabbitMQ.</param>
/// <param name="logger">Логгер для записи информации.</param>
public sealed class FakeOnUserAccountRegisteredConsumer(RabbitMqConnectionSource rabbitMq, Serilog.ILogger logger)
	: IConsumer
{
	/// <summary>
	/// Количество полученных сообщений.
	/// </summary>
	public static int Received;
	private const string Exchange = "identity";
	private const string Queue = "account.new";
	private const string RoutingKey = "account.new";

	private IChannel? _channel;
	private RabbitMqConnectionSource RabbitMq { get; } = rabbitMq;
	private Serilog.ILogger Logger { get; } = logger.ForContext<FakeOnUserAccountRegisteredConsumer>();
	private IChannel Channel => _channel ?? throw new NullReferenceException("Channel was not initialized");
	private AsyncEventHandler<BasicDeliverEventArgs> Handler =>
		async (_, @event) =>
		{
			string json = Encoding.UTF8.GetString(@event.Body.Span);
			Logger.Information("Received message {Json}", json);
			Received++;
			await Channel.BasicAckAsync(@event.DeliveryTag, false);
		};

	/// <summary>
	/// Инициализирует канал для потребления сообщений.
	/// </summary>
	/// <param name="connection">Подключение к RabbitMQ.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Инициализация канала для потребления сообщений.</returns>
	public async Task InitializeChannel(IConnection connection, CancellationToken ct = default) =>
		_channel = await TopicConsumerInitialization.InitializeChannel(RabbitMq, Exchange, Queue, RoutingKey, ct);

	/// <summary>
	/// Начинает потребление сообщений.
	/// </summary>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию начала потребления сообщений.</returns>
	public Task StartConsuming(CancellationToken ct = default)
	{
		AsyncEventingBasicConsumer consumer = new(Channel);
		consumer.ReceivedAsync += Handler;
		return Channel.BasicConsumeAsync(Queue, false, consumer, ct);
	}

	/// <summary>
	/// Останавливает потребление сообщений и освобождает ресурсы.
	/// </summary>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию остановки потребления сообщений и освобождения ресурсов.</returns>
	public async Task Shutdown(CancellationToken ct = default)
	{
		await Channel.CloseAsync(ct);
		await Channel.DisposeAsync();
	}
}
