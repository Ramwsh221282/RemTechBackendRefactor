namespace Identity.Domain.Tickets;

/// <summary>
/// Назначения заявок аккаунта.
/// </summary>
public static class AccountTicketPurposes
{
	/// <summary>
	/// Требуется подтверждение электронной почты.
	/// </summary>
	public const string EmailConfirmationRequired = "email-confirmation-required";

	/// <summary>
	/// Сброс пароля.
	/// </summary>
	public const string ResetPassword = "reset-password";
}
