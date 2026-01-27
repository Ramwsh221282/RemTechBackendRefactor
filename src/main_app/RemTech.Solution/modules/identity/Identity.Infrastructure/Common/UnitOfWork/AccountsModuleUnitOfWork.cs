using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Outbox;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Permissions;
using Identity.Domain.Tickets;

namespace Identity.Infrastructure.Common.UnitOfWork;

/// <summary>
/// Единица работы модуля аккаунтов.
/// </summary>
/// <param name="accounts">Трекер изменений аккаунтов.</param>
/// <param name="accountTickets">Трекер изменений тикетов аккаунтов.</param>
/// <param name="permissions">Трекер изменений разрешений.</param>
/// <param name="outboxMessages">Трекер изменений исходящих сообщений.</param>
public sealed class AccountsModuleUnitOfWork(
	AccountsChangeTracker accounts,
	AccountTicketsChangeTracker accountTickets,
	PermissionsChangeTracker permissions,
	IdentityOutboxMessageChangeTracker outboxMessages
) : IAccountsModuleUnitOfWork
{
	private AccountsChangeTracker Accounts { get; } = accounts;
	private AccountTicketsChangeTracker AccountTickets { get; } = accountTickets;
	private PermissionsChangeTracker Permissions { get; } = permissions;
	private IdentityOutboxMessageChangeTracker OutboxMessages { get; } = outboxMessages;

	/// <summary>
	/// Сохраняет изменения для коллекции аккаунтов.
	/// </summary>
	/// <param name="accounts">Коллекция аккаунтов для сохранения изменений.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию сохранения изменений.</returns>
	public Task Save(IEnumerable<Account> accounts, CancellationToken ct = default) =>
		Accounts.SaveChanges(accounts, ct);

	/// <summary>
	/// Сохраняет изменения для аккаунта.
	/// </summary>
	/// <param name="account">Аккаунт для сохранения изменений.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию сохранения изменений.</returns>
	public Task Save(Account account, CancellationToken ct = default) => Accounts.SaveChanges([account], ct);

	/// <summary>
	/// Сохраняет изменения для коллекции разрешений.
	/// </summary>
	/// <param name="permissions">Коллекция разрешений для сохранения изменений.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию сохранения изменений.</returns>
	public Task Save(IEnumerable<Permission> permissions, CancellationToken ct = default) =>
		Permissions.SaveChanges(permissions, ct);

	/// <summary>
	/// Сохраняет изменения для разрешения.
	/// </summary>
	/// <param name="permission">Разрешение для сохранения изменений.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию сохранения изменений.</returns>
	public Task Save(Permission permission, CancellationToken ct = default) =>
		Permissions.SaveChanges([permission], ct);

	/// <summary>
	/// Сохраняет изменения для коллекции тикетов аккаунтов.
	/// </summary>
	/// <param name="tickets">Коллекция тикетов аккаунтов для сохранения изменений.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию сохранения изменений.</returns>
	public Task Save(IEnumerable<AccountTicket> tickets, CancellationToken ct = default) =>
		AccountTickets.SaveChanges(tickets, ct);

	/// <summary>
	/// Сохраняет изменения для тикета аккаунта.
	/// </summary>
	/// <param name="ticket">Тикет аккаунта для сохранения изменений.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию сохранения изменений.</returns>
	public Task Save(AccountTicket ticket, CancellationToken ct = default) => AccountTickets.SaveChanges([ticket], ct);

	/// <summary>
	/// Сохраняет изменения для коллекции исходящих сообщений.
	/// </summary>
	/// <param name="messages">Коллекция исходящих сообщений для сохранения изменений.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию сохранения изменений.</returns>
	public Task Save(IEnumerable<IdentityOutboxMessage> messages, CancellationToken ct = default) =>
		OutboxMessages.Save(messages, ct);

	/// <summary>
	/// Сохраняет изменения для исходящего сообщения.
	/// </summary>
	/// <param name="message">Исходящее сообщение для сохранения изменений.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию сохранения изменений.</returns>
	public Task Save(IdentityOutboxMessage message, CancellationToken ct = default) =>
		OutboxMessages.Save([message], ct);

	/// <summary>
	/// Начинает отслеживание изменений для коллекции аккаунтов.
	/// </summary>
	/// <param name="accounts">Коллекция аккаунтов для отслеживания изменений.</param>
	public void Track(IEnumerable<Account> accounts) => Accounts.StartTracking(accounts);

	/// <summary>
	/// Начинает отслеживание изменений для коллекции тикетов аккаунтов.
	/// </summary>
	/// <param name="tickets">Коллекция тикетов аккаунтов для отслеживания изменений.</param>
	public void Track(IEnumerable<AccountTicket> tickets) => AccountTickets.StartTracking(tickets);

	/// <summary>
	/// Начинает отслеживание изменений для коллекции разрешений.
	/// </summary>
	/// <param name="permissions">Коллекция разрешений для отслеживания изменений.</param>
	public void Track(IEnumerable<Permission> permissions) => Permissions.StartTracking(permissions);

	/// <summary>
	/// Начинает отслеживание изменений для коллекции исходящих сообщений.
	/// </summary>
	/// <param name="messages">Коллекция исходящих сообщений для отслеживания изменений.</param>
	public void Track(IEnumerable<IdentityOutboxMessage> messages) => OutboxMessages.Track(messages);
}
