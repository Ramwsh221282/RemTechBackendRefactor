namespace Identity.Infrastructure.Accounts.Commands.ResetPassword;

/// <summary>
/// Сообщение, необходимое для сброса пароля.
/// </summary>
/// <param name="AccountId">Идентификатор аккаунта.</param>
/// <param name="AccountEmail">Электронная почта аккаунта.</param>
/// <param name="TicketId">Идентификатор тикета.</param>
/// <param name="TicketPurpose">Назначение тикета.</param>
public sealed record ResetPasswordRequiredMessage(
	Guid AccountId,
	string AccountEmail,
	Guid TicketId,
	string TicketPurpose
);
