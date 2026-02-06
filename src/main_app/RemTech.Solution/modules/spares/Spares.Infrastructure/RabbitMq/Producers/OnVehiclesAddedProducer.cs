using RemTech.SharedKernel.Infrastructure.RabbitMq;
using Spares.Domain.Features;

namespace Spares.Infrastructure.RabbitMq.Producers;

/// <summary>
/// Публикатор событий о добавлении запчастей в RabbitMQ.
/// </summary>
/// <param name="producer">Публикатор RabbitMQ.</param>
/// <param name="logger">Логгер для записи информации.</param>
public sealed class OnVehiclesAddedProducer(RabbitMqProducer producer, Serilog.ILogger logger)
	: IOnSparesAddedEventPublisher
{
	private const string EXCHANGE = "parsers";
	private const string ROUTING_KEY = "parsers.increase_processed";
	private RabbitMqProducer Producer { get; } = producer;
	private Serilog.ILogger Logger { get; } = logger.ForContext<OnVehiclesAddedProducer>();

	/// <summary>
	/// Публикует событие о добавлении запчастей.
	/// </summary>
	/// <param name="creatorId">Идентификатор создателя.</param>
	/// <param name="addedAmount">Количество добавленных запчастей.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию публикации.</returns>
	public async Task Publish(Guid creatorId, int addedAmount, CancellationToken ct = default)
	{
		OnSparesAddedMessage message = new() { Amount = addedAmount, Id = creatorId };
		RabbitMqPublishOptions options = new() { Persistent = true };
		await Producer.PublishDirectAsync(message, EXCHANGE, ROUTING_KEY, options, ct: ct);
		Logger.Information("Published {Id}. Amout: {Amount}", creatorId, addedAmount);
	}

	private sealed class OnSparesAddedMessage
	{
		public Guid Id { get; set; }
		public int Amount { get; set; }
	}
}
