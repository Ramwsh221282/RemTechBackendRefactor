using Identity.Contracts.Accounts;

namespace Identity.Application.Accounts.Messages;

public sealed record AccountActivationTicketRequired(Guid Id, string Email) : AccountMessage;