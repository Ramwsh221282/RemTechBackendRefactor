using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Outbox;
using Identity.Domain.Permissions;
using Identity.Domain.Tickets;

namespace Identity.Domain.Contracts.Persistence;

/// <summary>
/// Интерфейс единицы работы для модуля аккаунтов.
/// </summary>
public interface IAccountsModuleUnitOfWork
{
	/// <summary>
	/// Сохраняет изменения в коллекции аккаунтов.
	/// </summary>
	/// <param name="accounts">Коллекция аккаунтов для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача.</returns>
	Task Save(IEnumerable<Account> accounts, CancellationToken ct = default);

	/// <summary>
	/// Сохраняет изменения в аккаунте.
	/// </summary>
	/// <param name="account">Аккаунт для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача.</returns>
	Task Save(Account account, CancellationToken ct = default);

	/// <summary>
	/// Сохраняет изменения в коллекции разрешений.
	/// </summary>
	/// <param name="permissions">Коллекция разрешений для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача.</returns>
	Task Save(IEnumerable<Permission> permissions, CancellationToken ct = default);

	/// <summary>
	/// Сохраняет изменения в разрешении.
	/// </summary>
	/// <param name="permission">Разрешение для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача.</returns>
	Task Save(Permission permission, CancellationToken ct = default);

	/// <summary>
	/// Сохраняет изменения в коллекции тикетов аккаунтов.
	/// </summary>
	/// <param name="tickets">Коллекция тикетов аккаунтов для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача.</returns>
	Task Save(IEnumerable<AccountTicket> tickets, CancellationToken ct = default);

	/// <summary>
	/// Сохраняет изменения в тикете аккаунта.
	/// </summary>
	/// <param name="ticket">Тикет аккаунта для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача.</returns>
	Task Save(AccountTicket ticket, CancellationToken ct = default);

	/// <summary>
	/// Сохраняет изменения в коллекции сообщений исходящей очереди.
	/// </summary>
	/// <param name="messages">Коллекция сообщений исходящей очереди для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача.</returns>
	Task Save(IEnumerable<IdentityOutboxMessage> messages, CancellationToken ct = default);

	/// <summary>
	/// Сохраняет изменения в сообщении исходящей очереди.
	/// </summary>
	/// <param name="message">Сообщение исходящей очереди для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача.</returns>
	Task Save(IdentityOutboxMessage message, CancellationToken ct = default);

	/// <summary>
	/// Отслеживает изменения в коллекции аккаунтов.
	/// </summary>
	/// <param name="accounts">Коллекция аккаунтов для отслеживания изменений.</param>
	void Track(IEnumerable<Account> accounts);

	/// <summary>
	/// Отслеживает изменения в коллекции тикетов аккаунтов.
	/// </summary>
	/// <param name="tickets">Коллекция тикетов аккаунтов для отслеживания изменений.</param>
	void Track(IEnumerable<AccountTicket> tickets);

	/// <summary>
	/// Отслеживает изменения в коллекции разрешений.
	/// </summary>
	/// <param name="permissions">Коллекция разрешений для отслеживания изменений.</param>
	void Track(IEnumerable<Permission> permissions);

	/// <summary>
	/// Отслеживает изменения в коллекции сообщений исходящей очереди.
	/// </summary>
	/// <param name="messages">Коллекция сообщений исходящей очереди для отслеживания изменений.</param>
	void Track(IEnumerable<IdentityOutboxMessage> messages);
}
