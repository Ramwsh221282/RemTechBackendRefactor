using Identity.Domain.Tickets;
using RemTech.SharedKernel.Core.DomainEvents;
using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

namespace Identity.Domain.Accounts.Models.Events;

/// <summary>
/// Событие закрытия тикета аккаунтом.
/// </summary>
/// <param name="accountId">Идентификатор аккаунта.</param>
/// <param name="ticketId">Идентификатор тикета.</param>
public sealed class AccountClosedTicketEvent(Guid accountId, Guid ticketId) : IDomainEvent
{
	/// <summary>
	/// Создаёт событие закрытия тикета аккаунтом.
	/// </summary>
	/// <param name="account">Аккаунт, который закрыл тикет.</param>
	/// <param name="ticket">Тикет, который был закрыт.</param>
	public AccountClosedTicketEvent(Account account, AccountTicket ticket)
		: this(account.Id.Value, ticket.TicketId) { }

	/// <summary>
	/// Идентификатор аккаунта.
	/// </summary>
	public Guid AccountId { get; } = accountId;

	/// <summary>
	/// Идентификатор тикета.
	/// </summary>
	public Guid TicketId { get; } = ticketId;

	/// <summary>
	/// Публикация события на обработку.
	/// </summary>
	/// <param name="handler">Обработчик доменного события.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию обработки события.</returns>
	public Task PublishTo(IDomainEventHandler handler, CancellationToken ct = default)
	{
		return handler.Handle(this, ct);
	}
}
