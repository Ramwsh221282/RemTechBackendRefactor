using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace Notifications.Tests.Fakes;

/// <summary>
/// Фейковый издатель сообщений о регистрации аккаунта.
/// </summary>
/// <param name="producer">Публишер сообщений RabbitMQ.</param>
/// <param name="logger">Логгер для записи информации.</param>
public sealed class FakeAccountRegisteredPublisher(RabbitMqProducer producer, Serilog.ILogger logger)
{
	private const string EXCHANGE = "identity";
	private const string ROUTING_KEY = "account.new";

	private RabbitMqProducer Producer { get; } = producer;
	private Serilog.ILogger Logger { get; } = logger.ForContext<FakeAccountRegisteredPublisher>();

	/// <summary>
	/// Публикует сообщение о регистрации нового аккаунта.
	/// </summary>
	/// <param name="accountId">Идентификатор аккаунта.</param>
	/// <param name="ticketId">Идентификатор тикета.</param>
	/// <param name="email">Электронная почта пользователя.</param>
	/// <param name="login">Логин пользователя.</param>
	/// <returns>Опубликованная задача.</returns>
	public async Task Publish(Guid accountId, Guid ticketId, string email, string login)
	{
		RabbitMqPublishOptions options = new() { Persistent = true };
		NewAccountRegisteredOutboxMessagePayload payload = new(accountId, ticketId, email, login);
		await Producer.PublishDirectAsync(payload, EXCHANGE, ROUTING_KEY, options, CancellationToken.None);
		Logger.Information("Published account registration message for {Email}", email);
	}

	private sealed record NewAccountRegisteredOutboxMessagePayload(
		Guid AccountId,
		Guid TicketId,
		string Email,
		string Login
	);
}
