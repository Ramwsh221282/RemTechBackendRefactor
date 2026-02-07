using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using Spares.Domain.Features;

namespace Spares.Infrastructure.RabbitMq.Consumers;

/// <summary>
/// Потребитель сообщений о добавлении запчастей.
/// </summary>
/// <param name="rabbitMq">Источник подключения RabbitMQ.</param>
/// <param name="logger">Логгер для записи информации.</param>
/// <param name="services">Поставщик сервисов для разрешения зависимостей.</param>
public sealed class AddSparesConsumer(
	RabbitMqConnectionSource rabbitMq,
	Serilog.ILogger logger,
	IServiceProvider services
) : IConsumer
{
	private const string EXCHANGE = "spares";
	private const string QUEUE = "spares.add";
	private const string ROUTING_KEY = "spares.add";
	private IChannel? _channel;
	private IChannel Channel => _channel ?? throw new InvalidOperationException();
	private Serilog.ILogger Logger { get; } = logger.ForContext<AddSparesConsumer>();
	private IServiceProvider Services { get; } = services;
	private RabbitMqConnectionSource RabbitMq { get; } = rabbitMq;
	private AsyncEventHandler<BasicDeliverEventArgs> Handler =>
		async (_, @event) =>
		{
			Logger.Information("Received message to add spares.");
			try
			{
				AddSparesMessage message = AddSparesMessage.ConvertMessageFrom(@event);
				if (!IsMessageValid(message, out string error))
				{
					Logger.Error("Denied message: {Error}", error);
					await Channel.BasicAckAsync(@event.DeliveryTag, false);
					return;
				}

				AddSparesCommand command = CreateCommandFrom(message);
				(Guid creatorId, int added) = await SaveSpares(Services, command);
				Logger.Information("Added {Count} spares by {Id}", added, creatorId);
			}
			catch (Exception e)
			{
				Logger.Fatal(e, "Failed to process message.");
			}
			finally
			{
				// await Channel.BasicAckAsync(@event.DeliveryTag, false);
			}
		};

	/// <summary>
	/// Инициализирует канал для потребления сообщений.
	/// </summary>
	/// <param name="connection">Подключение к RabbitMQ.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инициализации канала.</returns>
	public async Task InitializeChannel(IConnection connection, CancellationToken ct = default)
	{
		_channel = await TopicConsumerInitialization.InitializeChannel(RabbitMq, EXCHANGE, QUEUE, ROUTING_KEY, ct);
	}

	/// <summary>
	/// Запускает потребление сообщений.
	/// </summary>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию начала потребления сообщений.</returns>
	public Task StartConsuming(CancellationToken ct = default)
	{
		AsyncEventingBasicConsumer consumer = new(Channel);
		consumer.ReceivedAsync += Handler;
		return Channel.BasicConsumeAsync(QUEUE, autoAck: false, consumer: consumer, cancellationToken: ct);
	}

	/// <summary>
	/// Останавливает потребление сообщений и закрывает канал.
	/// </summary>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию завершения работы канала.</returns>
	public Task Shutdown(CancellationToken ct = default)
	{
		return Channel.CloseAsync(ct);
	}

	private static async Task<(Guid CreatorId, int Added)> SaveSpares(
		IServiceProvider services,
		AddSparesCommand command
	)
	{
		await using AsyncServiceScope scope = services.CreateAsyncScope();
		return await scope
			.ServiceProvider.GetRequiredService<ICommandHandler<AddSparesCommand, (Guid, int)>>()
			.Execute(command);
	}

	private static AddSparesCommand CreateCommandFrom(AddSparesMessage message)
	{
		AddSparesCreatorPayload creator = CreateCreatorPayload(message);
		IEnumerable<AddSpareCommandPayload> spares = message.Payload.Select(ConvertToAddSpareCommandCreatorInfo);
		return new AddSparesCommand(creator, spares);
	}

	private static AddSparesCreatorPayload CreateCreatorPayload(AddSparesMessage message)
	{
		return new(
			CreatorId: message.CreatorId,
			CreatorDomain: message.CreatorDomain,
			CreatorType: message.CreatorType
		);
	}

	private static AddSpareCommandPayload ConvertToAddSpareCommandCreatorInfo(AddSpareMessagePayload payload)
	{
		return new(
			ContainedItemId: payload.ContainedItemId,
			Source: payload.Url,
			Oem: payload.Oem,
			Title: payload.Title,
			Price: payload.Price,
			IsNds: payload.IsNds,
			Address: payload.Address,
			Type: payload.Type,
			PhotoPaths: payload.Photos
		);
	}

	private static bool IsMessageValid(AddSparesMessage message, out string error)
	{
		List<string> errors = [];
		if (message.CreatorId == Guid.Empty)
		{
			errors.Add("Идентификатор создателя пуст");
		}

		if (string.IsNullOrWhiteSpace(message.CreatorType))
		{
			errors.Add("Тип создателя пуст");
		}

		if (string.IsNullOrWhiteSpace(message.CreatorDomain))
		{
			errors.Add("Домен создателя пуст");
		}

		if (message.Payload?.Any() != true)
		{
			errors.Add("Список запчастей пуст");
		}

		error = string.Join(", ", errors);
		return errors.Count == 0;
	}

	private sealed class AddSparesMessage
	{
		public Guid CreatorId { get; set; } = Guid.Empty;
		public string CreatorType { get; set; } = string.Empty;
		public string CreatorDomain { get; set; } = string.Empty;
		public IEnumerable<AddSpareMessagePayload> Payload { get; set; } = [];

		public static AddSparesMessage ConvertMessageFrom(BasicDeliverEventArgs @event)
		{
			return JsonSerializer.Deserialize<AddSparesMessage>(@event.Body.ToArray())!;
		}
	}

	private sealed class AddSpareMessagePayload
	{
		public Guid ContainedItemId { get; set; } = Guid.Empty;
		public string Address { get; set; } = string.Empty;
		public bool IsNds { get; set; }
		public string Oem { get; set; } = string.Empty;
		public string[] Photos { get; set; } = [];
		public long Price { get; set; }
		public string Url { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string Type { get; set; } = string.Empty;
	}
}
