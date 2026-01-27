namespace Identity.Domain.Accounts.Models;

/// <summary>
/// Типы сообщений для исходящей очереди аккаунта.
/// </summary>
public static class AccountOutboxMessageTypes
{
	/// <summary>
	/// Новое сообщение о создании аккаунта.
	/// </summary>
	public const string NewAccountCreated = "new-account-registered";
}
