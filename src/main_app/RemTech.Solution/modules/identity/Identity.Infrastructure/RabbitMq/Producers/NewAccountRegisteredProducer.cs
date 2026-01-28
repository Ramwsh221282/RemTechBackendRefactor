using System.Text.Json;
using Identity.Domain.Accounts.Features.RegisterAccount;
using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Outbox;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace Identity.Infrastructure.RabbitMq.Producers;

/// <summary>
/// Производитель сообщений о регистрации нового аккаунта.
/// </summary>
/// <param name="producer">Производитель RabbitMQ для отправки сообщений.   </param>
/// <param name="logger">Логгер для записи информации и ошибок.</param>
public sealed class NewAccountRegisteredProducer(RabbitMqProducer producer, Serilog.ILogger logger)
	: IAccountOutboxMessagePublisher
{
	private const string EXCHANGE = "identity";
	private const string ROUTING_KEY = "account.new";

	private RabbitMqProducer Producer { get; } = producer;
	private Serilog.ILogger Logger { get; } = logger.ForContext<NewAccountRegisteredProducer>();

	/// <summary>
	/// Проверяет, может ли данный тип сообщения быть опубликован этим производителем.
	/// </summary>
	/// <param name="message">Сообщение для проверки.</param>
	/// <returns>True, если сообщение может быть опубликовано, иначе false.</returns>
	public bool CanPublish(IdentityOutboxMessage message) =>
		message.Type == AccountOutboxMessageTypes.NEW_ACCOUNT_CREATED;

	/// <summary>
	/// Публикует сообщение о регистрации нового аккаунта в RabbitMQ.
	/// </summary>
	/// <param name="message">Сообщение для публикации.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию публикации сообщения.</returns>
	public async Task Publish(IdentityOutboxMessage message, CancellationToken ct = default)
	{
		NewAccountRegisteredOutboxMessagePayload payload = GetPayload(message);
		RabbitMqPublishOptions options = new() { Persistent = true };
		await Producer.PublishDirectAsync(payload, EXCHANGE, ROUTING_KEY, options, ct);
		message.MarkSent();
		Logger.Information("Published account registration message for {Email}", payload.Email);
	}

	private static NewAccountRegisteredOutboxMessagePayload GetPayload(IdentityOutboxMessage message) =>
		JsonSerializer.Deserialize<NewAccountRegisteredOutboxMessagePayload>(message.Payload)!;
}
