using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Core.Features.SubscribeParser;
using ParsersControl.Core.Parsers.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ParsersControl.Infrastructure.Listeners;

/// <summary>
/// Обработчик подписки парсера.
/// </summary>
/// <param name="services">Поставщик сервисов для создания областей обслуживания.</param>
/// <param name="logger">Логгер для записи информации.</param>
public sealed class ParserSubscribeConsumer(IServiceProvider services, Serilog.ILogger logger) : IConsumer
{
	private const string QUEUE = "parsers.create";
	private const string EXCHANGE = "parsers";
	private const string ROUTING_KEY = QUEUE;
	private IChannel? _channel;
	private Serilog.ILogger Logger { get; } = logger.ForContext<ParserSubscribeConsumer>();
	private IChannel Channel => _channel ?? throw new InvalidOperationException("Channel is not initialized.");
	private AsyncEventingBasicConsumer? Consumer { get; set; }

	/// <summary>
	/// Инициализирует канал для прослушивания сообщений.
	/// </summary>
	/// <param name="connection">Подключение к RabbitMQ.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию инициализации канала.</returns>
	public async Task InitializeChannel(IConnection connection, CancellationToken ct = default)
	{
		CreateChannelOptions options = new(
			publisherConfirmationsEnabled: true,
			publisherConfirmationTrackingEnabled: true
		);

		_channel = await connection.CreateChannelAsync(options, cancellationToken: ct);
		await DeclareExchange(_channel, ct);
		await DeclareQueue(_channel, ct);
		await BindQueue(_channel, ct);
		Consumer = CreateConsumer(_channel);
	}

	/// <summary>
	/// Начинает прослушивание сообщений.
	/// </summary>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию начала прослушивания сообщений.</returns>
	/// <exception cref="InvalidOperationException">Выбрасывается, если канал или потребитель не инициализированы.</exception>
	public Task StartConsuming(CancellationToken ct = default)
	{
		if (Channel is null)
			throw new InvalidOperationException("Channel is not initialized.");
		return Consumer is null
			? throw new InvalidOperationException("Consumer is not initialized.")
			: (Task)Channel.BasicConsumeAsync(queue: QUEUE, autoAck: false, consumer: Consumer, cancellationToken: ct);
	}

	/// <summary>
	/// Останавливает прослушивание сообщений и закрывает канал.
	/// </summary>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию остановки прослушивания сообщений.</returns>
	/// <exception cref="InvalidOperationException">Выбрасывается, если канал не инициализирован.</exception>
	public async Task Shutdown(CancellationToken ct = default)
	{
		if (Channel is null)
			throw new InvalidOperationException("Channel is not initialized.");
		await Channel.DisposeAsync();
	}

	private static Task DeclareExchange(IChannel channel, CancellationToken ct) =>
		channel.ExchangeDeclareAsync(
			exchange: EXCHANGE,
			type: "topic",
			durable: true,
			autoDelete: false,
			cancellationToken: ct
		);

	private static Task<QueueDeclareOk> DeclareQueue(IChannel channel, CancellationToken ct) =>
		channel.QueueDeclareAsync(
			queue: QUEUE,
			durable: true,
			exclusive: false,
			autoDelete: false,
			cancellationToken: ct
		);

	private static Task<Result<SubscribedParser>> HandleCommand(
		SubscribeParserCommand command,
		AsyncServiceScope scope
	) =>
		scope
			.ServiceProvider.GetRequiredService<ICommandHandler<SubscribeParserCommand, SubscribedParser>>()
			.Execute(command);

	private static Task BindQueue(IChannel channel, CancellationToken ct) =>
		channel.QueueBindAsync(queue: QUEUE, exchange: EXCHANGE, routingKey: ROUTING_KEY, cancellationToken: ct);

	private AsyncEventingBasicConsumer CreateConsumer(IChannel channel)
	{
		AsyncEventingBasicConsumer consumer = new(channel);
		consumer.ReceivedAsync += async (_, @event) =>
		{
			Logger.Information("Handling message.");
			ulong deliveryTag = @event.DeliveryTag;
			try
			{
				SubscribeParserMessage message = SubscribeParserMessage.Create(@event);
				await using AsyncServiceScope scope = services.CreateAsyncScope();
				SubscribeParserCommand command = new(message.parser_id, message.parser_domain, message.parser_type);
				Result<SubscribedParser> result = await HandleCommand(command, scope);

				if (result.IsFailure)
				{
					Logger.Error("Error at subscribing parser. {Error}", result.Error.Message);
				}

				await Channel.BasicAckAsync(deliveryTag, false);
			}
			catch (Exception ex)
			{
				Logger.Fatal(ex, "Failed to process message. DeliveryTag: {DeliveryTag}", deliveryTag);
				await Channel.BasicNackAsync(deliveryTag, false, true);
			}
		};

		return consumer;
	}

	private sealed class SubscribeParserMessage
	{
		public Guid parser_id { get; set; }
		public string parser_domain { get; set; } = null!;
		public string parser_type { get; set; } = null!;

		public static SubscribeParserMessage Create(BasicDeliverEventArgs @event) =>
			JsonSerializer.Deserialize<SubscribeParserMessage>(@event.Body.Span)!;
	}
}
