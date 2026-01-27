namespace Identity.Domain.Contracts.Outbox;

/// <summary>
/// Типы сообщений исходящей очереди.
/// </summary>
public static class OutboxMessageTypes
{
	/// <summary>
	/// Сообщение для подтверждения электронной почты.
	/// </summary>
	public const string EmailConfirmation = "email.confirmation";
}
