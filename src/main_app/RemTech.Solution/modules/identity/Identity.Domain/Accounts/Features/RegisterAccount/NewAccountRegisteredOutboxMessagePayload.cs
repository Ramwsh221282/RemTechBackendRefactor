using Identity.Domain.Contracts.Outbox;

namespace Identity.Domain.Accounts.Features.RegisterAccount;

/// <summary>
/// Параметры полезной нагрузки для сообщения о регистрации нового аккаунта.
/// </summary>
/// <param name="AccountId">Идентификатор аккаунта.</param>
/// <param name="TicketId">Идентификатор тикета.</param>
/// <param name="Email">Электронная почта пользователя.</param>
/// <param name="Login">Логин пользователя.</param>
public sealed record NewAccountRegisteredOutboxMessagePayload(Guid AccountId, Guid TicketId, string Email, string Login)
	: IOutboxMessagePayload;
