using Identity.Domain.Tickets;
using RemTech.SharedKernel.Core.DomainEvents;
using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

namespace Identity.Domain.Accounts.Models.Events;

public sealed class AccountClosedTicketEvent(Guid accountId, Guid ticketId) : IDomainEvent
{
    public AccountClosedTicketEvent(Account account, AccountTicket ticket)
        : this(account.Id.Value, ticket.TicketId) { }

    public Guid AccountId { get; } = accountId;
    public Guid TicketId { get; } = ticketId;

    public async Task PublishTo(IDomainEventHandler handler, CancellationToken ct = default) =>
        await handler.Handle(this, ct);
}
