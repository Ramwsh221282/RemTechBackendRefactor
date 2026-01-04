using Identity.Domain.Contracts;

namespace Identity.Domain.Accounts.Features.RegisterAccount;

public sealed record RegisteredOutboxMessagePayload(Guid AccountId, Guid TicketId, string Email, string Login) : IOutboxMessagePayload;