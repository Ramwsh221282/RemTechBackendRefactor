using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using Vehicles.Domain.Features.AddVehicle;

namespace Vehicles.Infrastructure.RabbitMq.Consumers;

/// <summary>
/// Потребитель сообщений для добавления транспортных средств.
/// </summary>
/// <param name="services">Сервис-провайдер для разрешения зависимостей.</param>
/// <param name="logger">Логгер для записи логов.</param>
/// <param name="rabbitMq">Источник подключения к RabbitMQ.</param>
public sealed class AddVehicleConsumer(
	IServiceProvider services,
	Serilog.ILogger logger,
	RabbitMqConnectionSource rabbitMq
) : IConsumer
{
	private const string EXCHANGE = "vehicles";
	private const string QUEUE = "vehicles.add";
	private const string ROUTING_KEY = "vehicles.add";

	private IChannel? _channel;
	private IChannel Channel => _channel ?? throw new InvalidOperationException("Channel was not initialized");
	private IServiceProvider Services { get; } = services;
	private Serilog.ILogger Logger { get; } = logger.ForContext<AddVehicleConsumer>();
	private RabbitMqConnectionSource RabbitMq { get; } = rabbitMq;
	private AsyncEventHandler<BasicDeliverEventArgs> Handler =>
		async (_, @event) =>
		{
			Logger.Information("Handling message.");
			try
			{
				AddVehicleMessage message = AddVehicleMessage.CreateFrom(@event);
				if (!IsMessageValid(message, out string error))
				{
					Logger.Error("Denied message: {Error}", error);
					await Channel.BasicAckAsync(@event.DeliveryTag, false);
					return;
				}

				AddVehicleCommand command = CreateCommandFrom(message);
				(Guid creatorId, int saved) result = await SaveVehicles(Services, command);
				Logger.Information("Vehicles added. Saved {Saved} by {Id}", result.saved, result.creatorId);
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "Error handling message.");
			}
			finally
			{
				await Channel.BasicAckAsync(@event.DeliveryTag, false);
			}
		};

	/// <summary>
	/// Инициализирует канал для потребления сообщений.
	/// </summary>
	/// <param name="connection">Подключение к RabbitMQ.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Инициализация канала.</returns>
	public async Task InitializeChannel(IConnection connection, CancellationToken ct = default) =>
		_channel = await TopicConsumerInitialization.InitializeChannel(RabbitMq, EXCHANGE, QUEUE, ROUTING_KEY, ct);

	/// <summary>
	/// Запускает потребление сообщений.
	/// </summary>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача потребления сообщений.</returns>
	public Task StartConsuming(CancellationToken ct = default)
	{
		AsyncEventingBasicConsumer consumer = new(Channel);
		consumer.ReceivedAsync += Handler;
		return Channel.BasicConsumeAsync(QUEUE, false, consumer, ct);
	}

	/// <summary>
	/// Останавливает потребление сообщений и закрывает канал.
	/// </summary>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача остановки потребления сообщений.</returns>
	public Task Shutdown(CancellationToken ct = default) => Channel.CloseAsync(ct);

	private static async Task<Result<(Guid CreatorId, int Saved)>> SaveVehicles(
		IServiceProvider services,
		AddVehicleCommand command
	)
	{
		await using AsyncServiceScope scope = services.CreateAsyncScope();
		return await scope
			.ServiceProvider.GetRequiredService<ICommandHandler<AddVehicleCommand, (Guid, int)>>()
			.Execute(command);
	}

	private static AddVehicleCommand CreateCommandFrom(AddVehicleMessage message)
	{
		AddVehicleCreatorCommandPayload creator = CreateCreatorPayload(message);
		IEnumerable<AddVehicleVehiclesCommandPayload> vehicles = CreateVehiclesPayload(message);
		return new AddVehicleCommand(creator, vehicles);
	}

	private static AddVehicleCreatorCommandPayload CreateCreatorPayload(AddVehicleMessage message) =>
		new(CreatorId: message.CreatorId, CreatorDomain: message.CreatorDomain, CreatorType: message.CreatorType);

	private static IEnumerable<AddVehicleVehiclesCommandPayload> CreateVehiclesPayload(AddVehicleMessage message) =>
		message.Payload.Select(p => new AddVehicleVehiclesCommandPayload(
			Id: p.Id,
			Title: p.Title,
			Url: p.Url,
			Price: p.Price,
			IsNds: p.IsNds,
			Address: p.Address,
			Photos: p.Photos,
			Characteristics: [.. p.Characteristics.Select(c => new AddVehicleCommandCharacteristics(c.Name, c.Value))]
		));

	private static bool IsMessageValid(AddVehicleMessage message, out string error)
	{
		List<string> errors = [];
		if (message.CreatorId == Guid.Empty)
			errors.Add("Идентификатор создателя пуст");
		if (string.IsNullOrWhiteSpace(message.CreatorType))
			errors.Add("Тип создателя пуст");
		if (string.IsNullOrWhiteSpace(message.CreatorDomain))
			errors.Add("Домен создателя пуст");
		if (message.Payload == null || !message.Payload.Any())
			errors.Add("Список автомобилей пуст");
		error = string.Join(", ", errors);
		return errors.Count == 0;
	}

	private sealed class AddVehicleMessage
	{
		public required Guid CreatorId { get; set; }
		public required string CreatorDomain { get; set; }
		public required string CreatorType { get; set; }
		public required IEnumerable<AddVehicleMessagePayload> Payload { get; set; }

		public static AddVehicleMessage CreateFrom(BasicDeliverEventArgs @event) =>
			JsonSerializer.Deserialize<AddVehicleMessage>(@event.Body.Span)!;
	}

	private sealed class AddVehicleMessagePayload
	{
		public required Guid Id { get; set; }
		public required string Title { get; set; }
		public required string Url { get; set; }
		public required long Price { get; set; }
		public required bool IsNds { get; set; }
		public required string Address { get; set; }
		public required string[] Photos { get; set; }
		public required IEnumerable<AddVehicleCharacteristic> Characteristics { get; set; }
	}

	private sealed class AddVehicleCharacteristic
	{
		public required string Name { get; set; }
		public required string Value { get; set; }
	}
}
