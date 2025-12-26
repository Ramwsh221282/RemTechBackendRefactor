using Identity.Contracts.Accounts;

namespace Identity.Application.Accounts.Messages;

public sealed record AccountPasswordResetRequired(Guid Id, string Email) : AccountMessage;