using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Accounts.ChangePassword;

public sealed record ChangeAccountPasswordRequest(Guid Id, string NewPassword, CancellationToken Ct) : IRequest;