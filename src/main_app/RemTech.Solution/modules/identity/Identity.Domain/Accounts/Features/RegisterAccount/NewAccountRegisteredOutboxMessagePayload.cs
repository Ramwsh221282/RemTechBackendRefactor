using Identity.Domain.Contracts;
using Identity.Domain.Contracts.Outbox;

namespace Identity.Domain.Accounts.Features.RegisterAccount;

public sealed record NewAccountRegisteredOutboxMessagePayload(
    Guid AccountId, 
    Guid TicketId, 
    string Email, 
    string Login)
    : IOutboxMessagePayload;