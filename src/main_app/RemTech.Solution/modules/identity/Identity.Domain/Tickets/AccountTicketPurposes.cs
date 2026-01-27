namespace Identity.Domain.Tickets;

/// <summary>
/// Назначения заявок аккаунта.
/// </summary>
public static class AccountTicketPurposes
{
	/// <summary>
	/// Требуется подтверждение электронной почты.
	/// </summary>
	public const string EMAIL_CONFIRMATION_REQUIRED = "email-confirmation-required";

	/// <summary>
	/// Сброс пароля.
	/// </summary>
	public const string RESET_PASSWORD = "reset-password";
}
