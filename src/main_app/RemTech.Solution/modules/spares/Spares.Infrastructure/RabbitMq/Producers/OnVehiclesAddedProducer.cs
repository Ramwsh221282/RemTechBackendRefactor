using RemTech.SharedKernel.Infrastructure.RabbitMq;
using Spares.Domain.Features;

namespace Spares.Infrastructure.RabbitMq.Producers;

public sealed class OnVehiclesAddedProducer(RabbitMqProducer producer, Serilog.ILogger logger)
	: IOnSparesAddedEventPublisher
{
	private const string Exchange = "parsers";
	private const string RoutingKey = "parsers.increase_processed";
	private RabbitMqProducer Producer { get; } = producer;
	private Serilog.ILogger Logger { get; } = logger.ForContext<OnVehiclesAddedProducer>();

	public async Task Publish(Guid creatorId, int addedAmount, CancellationToken ct = default)
	{
		OnSparesAddedMessage message = new() { Amount = addedAmount, Id = creatorId };
		RabbitMqPublishOptions options = new() { Persistent = true };
		await Producer.PublishDirectAsync(message, Exchange, RoutingKey, options, ct: ct);
		Logger.Information("Published {Id}. Amout: {Amount}", creatorId, addedAmount);
	}

	private sealed class OnSparesAddedMessage
	{
		public Guid Id { get; set; }
		public int Amount { get; set; }
	}
}
