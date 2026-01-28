using Identity.Domain.Accounts.Models;
using Identity.Domain.Tickets;

namespace Identity.Domain.Accounts.Features.ResetPassword;

/// <summary>
/// Результат сброса пароля пользователя.
/// </summary>
/// <param name="AccountId">Идентификатор аккаунта.</param>
/// <param name="AccountEmail">Электронная почта аккаунта.</param>
/// <param name="TicketId">Идентификатор тикета.</param>
/// <param name="TicketPurpose">Цель тикета.</param>
public sealed record ResetPasswordResult(Guid AccountId, string AccountEmail, Guid TicketId, string TicketPurpose)
{
	/// <summary>
	/// Создает результат сброса пароля из аккаунта и тикета.
	/// </summary>
	/// <param name="account">Аккаунт пользователя.</param>
	/// <param name="ticket">Тикет для сброса пароля.</param>
	/// <returns>Результат сброса пароля.</returns>
	public static ResetPasswordResult From(Account account, AccountTicket ticket) =>
		new(account.Id.Value, account.Email.Value, ticket.TicketId, ticket.Purpose);
}
