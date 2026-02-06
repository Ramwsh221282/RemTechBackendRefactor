using System.Text.Json;
using ContainedItems.Domain.Features.AddContainedItems;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ContainedItems.Infrastructure.Consumers;

/// <summary>
/// Потребитель для добавления содержащихся элементов.
/// </summary>
/// <param name="rabbitMq">Источник подключения к RabbitMQ.</param>
/// <param name="logger">Логгер для записи информации и ошибок.</param>
/// <param name="serviceProvider">Поставщик сервисов для разрешения зависимостей.</param>
public sealed class AddContainedItemsConsumer(
	RabbitMqConnectionSource rabbitMq,
	Serilog.ILogger logger,
	IServiceProvider serviceProvider
) : IConsumer
{
	private const string EXCHANGE = "contained-items";
	private const string QUEUE = "contained-items.create";
	private const string ROUTING_KEY = "contained-items.create";

	private IChannel? _channel;
	private Serilog.ILogger Logger { get; } = logger.ForContext<AddContainedItemsConsumer>();
	private IServiceProvider Services { get; } = serviceProvider;
	private RabbitMqConnectionSource RabbitMq { get; } = rabbitMq;
	private IChannel Channel => _channel ?? throw new InvalidOperationException("Channel was not initialized");
	private AsyncEventHandler<BasicDeliverEventArgs> Handler =>
		async (_, @event) =>
		{
			Logger.Information("Handling message");
			try
			{
				AddContainedItemsMessage message = AddContainedItemsMessage.FromEventArgs(@event);
				IEnumerable<AddContainedItemsBody> commandPayload = message.Items.Select(i =>
				{
					return new AddContainedItemsBody(
						ServiceItemId: i.ItemId,
						ItemType: i.ItemType,
						CreatorId: message.CreatorId,
						CreatorDomain: message.CreatorDomain,
						CreatorType: message.CreatorType,
						Content: i.Content
					);
				});

				await using AsyncServiceScope scope = Services.CreateAsyncScope();
				AddContainedItemsCommand command = new(commandPayload);
				Result<int> inserted = await scope
					.ServiceProvider.GetRequiredService<ICommandHandler<AddContainedItemsCommand, int>>()
					.Execute(command);

				if (inserted.IsSuccess)
				{
					Logger.Information("Added {Count} items.", inserted.Value);
				}

				await Channel.BasicAckAsync(@event.DeliveryTag, false);
			}
			catch (Exception e)
			{
				Logger.Fatal(e, "Failed to process message.");
				await Channel.BasicNackAsync(@event.DeliveryTag, false, false);
			}
		};

	/// <summary>
	/// Инициализирует канал для потребления сообщений.
	/// </summary>
	/// <param name="connection">Подключение к RabbitMQ.</param>
	/// <param name="ct">Токен отмены для прерывания операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инициализации канала.</returns>
	public async Task InitializeChannel(IConnection connection, CancellationToken ct = default)
	{
		_channel = await TopicConsumerInitialization.InitializeChannel(RabbitMq, EXCHANGE, QUEUE, ROUTING_KEY, ct);
	}

	/// <summary>
	/// Запускает потребление сообщений.
	/// </summary>
	/// <param name="ct">Токен отмены для прерывания операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию запуска потребления сообщений.</returns>
	public Task StartConsuming(CancellationToken ct = default)
	{
		AsyncEventingBasicConsumer consumer = new(Channel);
		consumer.ReceivedAsync += Handler;
		return Channel.BasicConsumeAsync(QUEUE, autoAck: false, consumer: consumer, cancellationToken: ct);
	}

	/// <summary>
	/// Останавливает потребление сообщений и закрывает канал.
	/// </summary>
	/// <param name="ct">Токен отмены для прерывания операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию остановки потребления сообщений и закрытия канала.</returns>
	public Task Shutdown(CancellationToken ct = default)
	{
		return Channel.CloseAsync(cancellationToken: ct);
	}

	private sealed class AddContainedItemsMessage
	{
		public Guid CreatorId { get; set; } = Guid.Empty;
		public string CreatorDomain { get; set; } = string.Empty;
		public string CreatorType { get; set; } = string.Empty;
		public AddContainedItemMessagePayload[] Items { get; set; } = [];

		public static AddContainedItemsMessage FromEventArgs(BasicDeliverEventArgs ea)
		{
			return JsonSerializer.Deserialize<AddContainedItemsMessage>(ea.Body.Span)!;
		}
	}

	private sealed class AddContainedItemMessagePayload
	{
		public string ItemType { get; set; } = string.Empty;
		public string ItemId { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;
	}
}
