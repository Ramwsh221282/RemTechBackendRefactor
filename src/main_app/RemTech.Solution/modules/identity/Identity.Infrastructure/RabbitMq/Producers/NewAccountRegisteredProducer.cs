using System.Text.Json;
using Identity.Domain.Accounts.Features.RegisterAccount;
using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Outbox;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace Identity.Infrastructure.RabbitMq.Producers;

public sealed class NewAccountRegisteredProducer(RabbitMqProducer producer, Serilog.ILogger logger)
	: IAccountOutboxMessagePublisher
{
	private const string Exchange = "identity";
	private const string RoutingKey = "account.new";

	private RabbitMqProducer Producer { get; } = producer;
	private Serilog.ILogger Logger { get; } = logger.ForContext<NewAccountRegisteredProducer>();

	public bool CanPublish(IdentityOutboxMessage message) =>
		message.Type == AccountOutboxMessageTypes.NewAccountCreated;

	public async Task Publish(IdentityOutboxMessage message, CancellationToken ct = default)
	{
		NewAccountRegisteredOutboxMessagePayload payload = GetPayload(message);
		RabbitMqPublishOptions options = new() { Persistent = true };
		await Producer.PublishDirectAsync(payload, Exchange, RoutingKey, options, ct);
		message.MarkSent();
		Logger.Information("Published account registration message for {Email}", payload.Email);
	}

	private static NewAccountRegisteredOutboxMessagePayload GetPayload(IdentityOutboxMessage message) =>
		JsonSerializer.Deserialize<NewAccountRegisteredOutboxMessagePayload>(message.Payload)!;
}
