using RemTech.SharedKernel.Infrastructure.RabbitMq;
using Vehicles.Domain.Features.AddVehicle;

namespace Vehicles.Infrastructure.RabbitMq.Producers;

/// <summary>
/// Публикатор событий о добавлении транспортных средств.
/// </summary>
/// <param name="producer">Публикатор RabbitMQ.</param>
/// <param name="logger">Логгер Serilog.</param>
public sealed class OnVehiclesAddedProducer(RabbitMqProducer producer, Serilog.ILogger logger)
	: IOnVehiclesAddedEventPublisher
{
	private const string EXCHANGE = "parsers";
	private const string ROUTING_KEY = "parsers.increase_processed";
	private RabbitMqProducer Producer { get; } = producer;
	private Serilog.ILogger Logger { get; } = logger.ForContext<OnVehiclesAddedProducer>();

	/// <summary>
	/// Публикует событие о добавлении транспортных средств.
	/// </summary>
	/// <param name="creatorId">Идентификатор создателя.</param>
	/// <param name="addedAmount">Количество добавленных транспортных средств.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию публикации.</returns>
	public async Task Publish(Guid creatorId, int addedAmount, CancellationToken ct = default)
	{
		OnVehiclesAddedMessage message = new() { Id = creatorId, Amount = addedAmount };
		RabbitMqPublishOptions options = new() { Persistent = true };
		await Producer.PublishDirectAsync(message, EXCHANGE, ROUTING_KEY, options, ct: ct);
		Logger.Information("Published {Id}. Amout: {Amount}", creatorId, addedAmount);
	}

	private sealed class OnVehiclesAddedMessage
	{
		public Guid Id { get; set; }
		public int Amount { get; set; }
	}
}
