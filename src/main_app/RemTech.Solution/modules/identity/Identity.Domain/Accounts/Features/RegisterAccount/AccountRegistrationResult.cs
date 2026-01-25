using Identity.Domain.Contracts.Outbox;

namespace Identity.Domain.Accounts.Features.RegisterAccount;

public sealed record AccountRegistrationResult(Guid AccountId, Guid TicketId, string Email, string Login)
	: IOutboxMessagePayload;
