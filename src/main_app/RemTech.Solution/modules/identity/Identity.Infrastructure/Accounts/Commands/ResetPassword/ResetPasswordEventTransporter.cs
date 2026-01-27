using Identity.Domain.Accounts.Features.ResetPassword;
using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace Identity.Infrastructure.Accounts.Commands.ResetPassword;

/// <summary>
/// Транспортировщик событий для команды сброса пароля.
/// </summary>
/// <param name="producer">Производитель сообщений RabbitMQ.</param>
/// <param name="logger">Логгер для записи информации.</param>
public sealed class ResetPasswordEventTransporter(RabbitMqProducer producer, Serilog.ILogger logger)
	: IEventTransporter<ResetPasswordCommand, ResetPasswordResult>
{
	private const string Exchange = "accounts";
	private Serilog.ILogger Logger { get; } = logger.ForContext<ResetPasswordCommand>();
	private RabbitMqProducer Producer { get; } = producer;

	/// <summary>
	/// Транспортирует событие сброса пароля.
	/// </summary>
	/// <param name="result">Результат выполнения команды сброса пароля.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию.</returns>
	public async Task Transport(ResetPasswordResult result, CancellationToken ct = default)
	{
		ResetPasswordRequiredMessage message = MapToMessage(result);
		await PublishMessageToRabbitMq(message, ct);

		Logger.Information(
			"Published ResetPasswordRequiredMessage to RabbitMQ for AccountId: {AccountId} and Email: {Email}",
			result.AccountId,
			result.AccountEmail
		);
	}

	private static ResetPasswordRequiredMessage MapToMessage(ResetPasswordResult result) =>
		new(
			AccountId: result.AccountId,
			AccountEmail: result.AccountEmail,
			TicketId: result.TicketId,
			TicketPurpose: result.TicketPurpose
		);

	private Task PublishMessageToRabbitMq(ResetPasswordRequiredMessage message, CancellationToken ct)
	{
		RabbitMqPublishOptions options = new() { Persistent = true };
		return Producer.PublishDirectAsync(message, Exchange, message.TicketPurpose, options, ct);
	}
}
