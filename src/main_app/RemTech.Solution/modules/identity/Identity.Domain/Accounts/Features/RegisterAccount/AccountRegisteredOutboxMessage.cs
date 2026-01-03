using Identity.Domain.Contracts;

namespace Identity.Domain.Accounts.Features.RegisterAccount;

public sealed record AccountRegisteredOutboxMessage(Guid AccountId, Guid TicketId, string Email, string Login) : IAccountOutboxMessage;