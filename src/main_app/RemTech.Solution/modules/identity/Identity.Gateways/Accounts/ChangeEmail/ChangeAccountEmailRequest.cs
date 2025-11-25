using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Accounts.ChangeEmail;

public sealed record ChangeAccountEmailRequest(Guid Id, string Email, CancellationToken Ct) : IRequest;