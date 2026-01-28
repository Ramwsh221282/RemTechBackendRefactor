using RemTech.SharedKernel.Core.DomainEvents;
using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

namespace Identity.Domain.Accounts.Models.Events;

/// <summary>
/// Событие создания нового аккаунта.
/// </summary>
/// <param name="accountId">Идентификатор аккаунта.</param>
/// <param name="accountEmail">Email аккаунта.</param>
/// <param name="accountLogin">Логин аккаунта.</param>
public sealed class NewAccountCreatedEvent(Guid accountId, string accountEmail, string accountLogin) : IDomainEvent
{
	/// <summary>
	/// Создаёт событие создания нового аккаунта.
	/// </summary>
	/// <param name="account">Аккаунт, который был создан.</param>
	public NewAccountCreatedEvent(Account account)
		: this(account.Id.Value, account.Email.Value, account.Login.Value) { }

	/// <summary>
	/// Идентификатор аккаунта.
	/// </summary>
	public Guid AccountId { get; private set; } = accountId;

	/// <summary>
	/// Email аккаунта.
	/// </summary>
	public string AccountEmail { get; private set; } = accountEmail;

	/// <summary>
	/// Логин аккаунта.
	/// </summary>
	public string AccountLogin { get; private set; } = accountLogin;

	/// <summary>
	/// Публикация события на обработку.
	/// </summary>
	/// <param name="handler">Обработчик доменного события.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию обработки события.</returns>
	public Task PublishTo(IDomainEventHandler handler, CancellationToken ct = default) => handler.Handle(this, ct);
}
